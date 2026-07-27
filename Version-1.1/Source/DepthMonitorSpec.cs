using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Calloatti.DepthMonitor
{
  internal record DepthMonitorSpec : ComponentSpec
  {
    [Serialize]
    public Vector3Int SensorCoordinates { get; init; }

    [Serialize]
    public float SensorHeightOffset { get; init; }
  }
}