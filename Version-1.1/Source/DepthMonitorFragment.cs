using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Calloatti.DepthMonitor
{
  internal class DepthMonitorFragment : IEntityPanelFragment
  {
    private static readonly float ThresholdChangeStep = 0.01f;

    private readonly VisualElementLoader _visualElementLoader;
    private readonly ILoc _loc;

    private DepthMonitor _depthMonitor;
    private VisualElement _root;
    private Label _measurement;

    // ON Controls
    private Label _thresholdOnLabel;
    private PreciseSlider _thresholdOnSlider;
    private Label _onTitleLabel;

    // OFF Controls
    private Label _thresholdOffLabel;
    private PreciseSlider _thresholdOffSlider;
    private Label _offTitleLabel;

    // Localization keys for titles
    private static readonly string TurnOnIfLocKey = "Building.DepthMonitor.TurnOnIf";
    private static readonly string TurnOffIfLocKey = "Building.DepthMonitor.TurnOffIf";

    // MIMIC VANILLA: Added explicit "F2" to format distances to exactly 2 decimal places
    private readonly Phrase _measurementPhrase = Phrase.New("Automation.Measurement").FormatDistance<float>("F2");
    private readonly Phrase _thresholdPhrase = Phrase.New("Automation.Threshold").FormatDistance<float>("F2");

    public DepthMonitorFragment(VisualElementLoader visualElementLoader, ILoc loc)
    {
      _visualElementLoader = visualElementLoader;
      _loc = loc;
    }

    public VisualElement InitializeFragment()
    {
      _root = _visualElementLoader.LoadVisualElement("DepthMonitor/DepthMonitorFragment");

      _measurement = _root.Q<Label>("Measurement");
      
      _onTitleLabel = _root.Q<Label>("OnTitle");
      _onTitleLabel.text = _loc.T(TurnOnIfLocKey);
      
      _thresholdOnLabel = _root.Q<Label>("ThresholdOnLabel");
      _thresholdOnSlider = _root.Q<PreciseSlider>("ThresholdOnSlider");
      _thresholdOnSlider.SetValueChangedCallback(OnThresholdOnChanged);
      _thresholdOnSlider.SetStepWithoutNotify(ThresholdChangeStep);

      _offTitleLabel = _root.Q<Label>("OffTitle");
      _offTitleLabel.text = _loc.T(TurnOffIfLocKey);
      
      _thresholdOffLabel = _root.Q<Label>("ThresholdOffLabel");
      _thresholdOffSlider = _root.Q<PreciseSlider>("ThresholdOffSlider");
      _thresholdOffSlider.SetValueChangedCallback(OnThresholdOffChanged);
      _thresholdOffSlider.SetStepWithoutNotify(ThresholdChangeStep);

      _root.ToggleDisplayStyle(false);
      return _root;
    }

    public void ShowFragment(BaseComponent entity)
    {
      _depthMonitor = entity.GetComponent<DepthMonitor>();
    }

    public void ClearFragment()
    {
      _depthMonitor = null;
      _root.ToggleDisplayStyle(false);
    }

    public void UpdateFragment()
    {
      _root.ToggleDisplayStyle(_depthMonitor != null);
      if (_depthMonitor != null)
      {
        _measurement.text = _loc.T(_measurementPhrase, _depthMonitor.DepthFromFloor);

        _thresholdOnLabel.text = _loc.T(_thresholdPhrase, _depthMonitor.ThresholdOnFromFloor);
        _thresholdOnSlider.UpdateValuesWithoutNotify(_depthMonitor.ThresholdOn, _depthMonitor.MinThreshold, _depthMonitor.MaxThreshold);
        _thresholdOnSlider.SetMarker(_depthMonitor.Depth);

        _thresholdOffLabel.text = _loc.T(_thresholdPhrase, _depthMonitor.ThresholdOffFromFloor);
        _thresholdOffSlider.UpdateValuesWithoutNotify(_depthMonitor.ThresholdOff, _depthMonitor.MinThreshold, _depthMonitor.MaxThreshold);
        _thresholdOffSlider.SetMarker(_depthMonitor.Depth);
      }
    }

    private void OnThresholdOnChanged(float value)
    {
      // Clean up floating point inaccuracies from the slider delta before setting
      float roundedValue = (float)Math.Round(value, 2);
      _depthMonitor.SetThresholdOn(roundedValue);
    }

    private void OnThresholdOffChanged(float value)
    {
      // Clean up floating point inaccuracies from the slider delta before setting
      float roundedValue = (float)Math.Round(value, 2);
      _depthMonitor.SetThresholdOff(roundedValue);
    }
  }
}