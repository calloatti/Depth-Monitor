using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.WaterSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Calloatti.DepthMonitor
{
  // MINIMAL 1.1 FIX: Removed ONLY the deleted 'IStartableComponent' interface. Left everything else exactly as you wrote it.
  public class DepthMonitor : BaseComponent, IAwakableComponent, IInitializableEntity, IPersistentEntity, IDuplicable<DepthMonitor>, IDuplicable, ISamplingTransmitter, ITransmitter
  {
    private static readonly ComponentKey DepthMonitorKey = new ComponentKey("DepthMonitor");

    private static readonly PropertyKey<float> ThresholdOnKey = new PropertyKey<float>("ThresholdOn");
    private static readonly PropertyKey<float> ThresholdOffKey = new PropertyKey<float>("ThresholdOff");
    private static readonly PropertyKey<bool> CurrentlyActiveKey = new PropertyKey<bool>("CurrentlyActive");

    // Default offsets if placed fresh
    private static readonly float DefaultDepthOffsetOn = 0.2f;
    private static readonly float DefaultDepthOffsetOff = 0.8f;

    private readonly IThreadSafeWaterMap _threadSafeWaterMap;

    private Automator _automator;
    private BlockObject _blockObject;
    private DepthMonitorSpec _depthMonitorSpec;

    private float? _rawThresholdOn;
    private float? _rawThresholdOff;
    private bool _currentlyActive;

    private float _sampledWaterHeight;
    private int _sampledFloor;

    public Vector3Int SensorCoordinates { get; private set; }

    public int MinThreshold => _sampledFloor;
    public float MaxThreshold => (float)SensorCoordinates.z + _depthMonitorSpec.SensorHeightOffset;

    public float ThresholdOn => Mathf.Clamp(_rawThresholdOn ?? 0f, MinThreshold, MaxThreshold);
    public float ThresholdOnFromFloor => ThresholdOn - (float)_sampledFloor;

    public float ThresholdOff => Mathf.Clamp(_rawThresholdOff ?? 0f, MinThreshold, MaxThreshold);
    public float ThresholdOffFromFloor => ThresholdOff - (float)_sampledFloor;

    public float Depth => Mathf.Clamp(_sampledWaterHeight, MinThreshold, MaxThreshold);
    public float DepthFromFloor => Mathf.Clamp(_sampledWaterHeight - (float)_sampledFloor, 0f, MaxThreshold - (float)MinThreshold);

    public DepthMonitor(IThreadSafeWaterMap threadSafeWaterMap)
    {
      _threadSafeWaterMap = threadSafeWaterMap;
    }

    public void Awake()
    {
      _automator = GetComponent<Automator>();
      _blockObject = GetComponent<BlockObject>();
      _depthMonitorSpec = GetComponent<DepthMonitorSpec>();
    }

    public void InitializeEntity()
    {
      InitializeSensorCoordinates();

      if (!_rawThresholdOn.HasValue)
      {
        _rawThresholdOn = (float)SensorCoordinates.z + DefaultDepthOffsetOn;
      }
      if (!_rawThresholdOff.HasValue)
      {
        _rawThresholdOff = (float)SensorCoordinates.z + DefaultDepthOffsetOff;
      }
    }

    public void Start()
    {
      _automator?.SetState(_currentlyActive);
    }

    public void SetThresholdOn(float value)
    {
      if (!_rawThresholdOn.Equals(value))
      {
        _rawThresholdOn = value;
        UpdateOutputState();
      }
    }

    public void SetThresholdOff(float value)
    {
      if (!_rawThresholdOff.Equals(value))
      {
        _rawThresholdOff = value;
        UpdateOutputState();
      }
    }

    public void Save(IEntitySaver entitySaver)
    {
      IObjectSaver component = entitySaver.GetComponent(DepthMonitorKey);

      if (_rawThresholdOn.HasValue) component.Set(ThresholdOnKey, _rawThresholdOn.Value);
      if (_rawThresholdOff.HasValue) component.Set(ThresholdOffKey, _rawThresholdOff.Value);

      component.Set(CurrentlyActiveKey, _currentlyActive);
    }

    public void Load(IEntityLoader entityLoader)
    {
      if (entityLoader.TryGetComponent(DepthMonitorKey, out var component))
      {
        if (component.Has(ThresholdOnKey)) _rawThresholdOn = component.Get(ThresholdOnKey);
        if (component.Has(ThresholdOffKey)) _rawThresholdOff = component.Get(ThresholdOffKey);
        if (component.Has(CurrentlyActiveKey)) _currentlyActive = component.Get(CurrentlyActiveKey);
      }
    }

    public void DuplicateFrom(DepthMonitor source)
    {
      InitializeSensorCoordinates();
      _rawThresholdOn = (float)GetCurrentFloor() + source.ThresholdOnFromFloor;
      _rawThresholdOff = (float)GetCurrentFloor() + source.ThresholdOffFromFloor;
      _currentlyActive = source._currentlyActive;
      UpdateOutputState();
    }

    public void Sample()
    {
      _sampledWaterHeight = _threadSafeWaterMap.WaterHeightOrFloor(SensorCoordinates);
      _sampledFloor = GetCurrentFloor();
      UpdateOutputState();
    }

    private void InitializeSensorCoordinates()
    {
      SensorCoordinates = _blockObject.TransformCoordinates(_depthMonitorSpec.SensorCoordinates);
    }

    private void UpdateOutputState()
    {
      bool targetState = _currentlyActive;

      // Turn ON if the water goes below or equals the low threshold
      if (_sampledWaterHeight <= ThresholdOn) targetState = true;

      // Turn OFF if the water goes above or equals the high threshold
      if (_sampledWaterHeight >= ThresholdOff) targetState = false;

      if (targetState != _currentlyActive)
      {
        _currentlyActive = targetState;
        _automator?.SetState(_currentlyActive);
      }
    }

    private int GetCurrentFloor()
    {
      if (!_threadSafeWaterMap.TryGetColumnFloor(SensorCoordinates, out var floor))
      {
        return SensorCoordinates.z;
      }
      return floor;
    }
  }
}