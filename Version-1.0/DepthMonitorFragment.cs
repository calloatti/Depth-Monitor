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

    // Custom Localizations Keys
    private static readonly string TurnOnIfLocKey = "Building.DepthMonitor.TurnOnIf";
    private static readonly string TurnOffIfLocKey = "Building.DepthMonitor.TurnOffIf";

    private readonly VisualElementLoader _visualElementLoader;
    private readonly ILoc _loc;

    private DepthMonitor _depthMonitor;
    private VisualElement _root;
    private Label _measurement;

    // ON Controls
    private Label _thresholdOnLabel;
    private PreciseSlider _thresholdOnSlider;

    // OFF Controls
    private Label _thresholdOffLabel;
    private PreciseSlider _thresholdOffSlider;

    // FIX: Updated to use the new extension method syntax for current Timberborn UIFormatters
    private readonly Phrase _measurementPhrase = Phrase.New("Automation.Measurement").FormatDistance<float>();
    private readonly Phrase _thresholdPhrase = Phrase.New("Automation.Threshold").FormatDistance<float>();

    public DepthMonitorFragment(VisualElementLoader visualElementLoader, ILoc loc)
    {
      _visualElementLoader = visualElementLoader;
      _loc = loc;
    }

    public VisualElement InitializeFragment()
    {
      // We load the vanilla fragment as a base, but we will hack out the pieces we don't need and clone the ones we do.
      _root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/WaterSensorFragment");

      _measurement = _root.Q<Label>("Measurement");

      // Hide the unneeded Comparison dropdown completely
      var modeDropdown = _root.Q<VisualElement>("Mode");
      if (modeDropdown != null) modeDropdown.ToggleDisplayStyle(false);

      // 1. Prepare ON Controls (Reusing original slider and label)
      _thresholdOnLabel = _root.Q<Label>("ThresholdLabel");
      _thresholdOnSlider = _root.Q<PreciseSlider>("ThresholdSlider");
      _thresholdOnSlider.SetValueChangedCallback(OnThresholdOnChanged);
      _thresholdOnSlider.SetStepWithoutNotify(ThresholdChangeStep);

      var onTitle = new Label(_loc.T(TurnOnIfLocKey));
      onTitle.AddToClassList("game-text-normal");
      onTitle.style.marginTop = 10;
      _root.Insert(_root.IndexOf(_thresholdOnLabel), onTitle);

      // 2. Prepare OFF Controls (Extracting from a new instance of the template)
      var offTemplate = _visualElementLoader.LoadVisualElement("Game/EntityPanel/WaterSensorFragment");

      _thresholdOffLabel = offTemplate.Q<Label>("ThresholdLabel");
      _thresholdOffSlider = offTemplate.Q<PreciseSlider>("ThresholdSlider");
      _thresholdOffSlider.SetValueChangedCallback(OnThresholdOffChanged);
      _thresholdOffSlider.SetStepWithoutNotify(ThresholdChangeStep);

      var offTitle = new Label(_loc.T(TurnOffIfLocKey));
      offTitle.AddToClassList("game-text-normal");
      offTitle.style.marginTop = 10;

      _root.Add(offTitle);
      _root.Add(_thresholdOffLabel);
      _root.Add(_thresholdOffSlider);

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
      _depthMonitor.SetThresholdOn(value);
    }

    private void OnThresholdOffChanged(float value)
    {
      _depthMonitor.SetThresholdOff(value);
    }
  }
}