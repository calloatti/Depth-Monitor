using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Calloatti.DepthMonitor
{
  // Converted from a 'record' inheriting 'ComponentSpec' to a standard 'class' 
  // inheriting 'BaseComponent' to completely bypass the publicized assembly catch-22.
  public class DepthMonitorSpec : BaseComponent
  {
    [Serialize]
    public Vector3Int SensorCoordinates { get; init; }

    [Serialize]
    public float SensorHeightOffset { get; init; }
  }
}