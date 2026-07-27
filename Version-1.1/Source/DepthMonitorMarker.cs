using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Rendering;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Calloatti.DepthMonitor
{
  internal class DepthMonitorMarker : BaseComponent, IAwakableComponent, IUpdatableComponent, ISelectionListener
  {
    // Custom colors to help tell the markers apart
    private static readonly Color32 MarkerOnColor = Color.green;
    private static readonly Color32 MarkerOffColor = Color.red;
    private static readonly float MarkerYOffset = 0.02f;

    private readonly MarkerDrawerFactory _markerDrawerFactory;

    private DepthMonitor _depthMonitor;
    private MeshDrawer _markerOnDrawer;
    private MeshDrawer _markerOffDrawer;
    private BlockObject _blockObject;

    public DepthMonitorMarker(MarkerDrawerFactory markerDrawerFactory)
    {
      _markerDrawerFactory = markerDrawerFactory;
    }

    public void Awake()
    {
      _depthMonitor = GetComponent<DepthMonitor>();
      _blockObject = GetComponent<BlockObject>();

      _markerOnDrawer = _markerDrawerFactory.CreateTileDrawer(MarkerOnColor);
      _markerOffDrawer = _markerDrawerFactory.CreateTileDrawer(MarkerOffColor);
      DisableComponent();
    }

    public void Update()
    {
      Vector3Int sensorCoordinates = _depthMonitor.SensorCoordinates;
      Vector3Int renderCoords = new Vector3Int(sensorCoordinates.x, sensorCoordinates.y, 0);

      _markerOnDrawer.DrawAtCoordinates(renderCoords, _depthMonitor.ThresholdOn + MarkerYOffset);
      _markerOffDrawer.DrawAtCoordinates(renderCoords, _depthMonitor.ThresholdOff + MarkerYOffset);
    }

    public void OnSelect()
    {
      if (!_blockObject.IsPreview)
      {
        EnableComponent();
      }
    }

    public void OnUnselect()
    {
      DisableComponent();
    }
  }
}