Include ..\AGENTS.md

# Depth Monitor — Mod-Specific Agent Instructions

## Identity
- **Assembly:** `depthmonitor`
- **Namespace:** `Calloatti.DepthMonitor`
- **ModId:** `Calloatti.DepthMonitor`
- **Framework:** Bindito DI
- **Publicizer:** `Timberborn.BlueprintSystem` is publicized via `CommonModSettings.props`, with `DoNotPublicize` for `ComponentSpec.EqualityContract`/`PrintMembers` (record-inheritance CS0507 fix — see csproj)
- **Min Game Version:** 1.0.12.9 — uses `timberborn-decompiled-1.0.*`

## What This Mod Does
Adds a depth monitor building that displays water depth information. Includes marker placement on water tiles with a custom entity panel fragment for data display.

## Source Architecture (`Version-1.0/Source/`)

| File | Role |
|---|---|
| `DepthMonitor.cs` | Core depth monitor component |
| `DepthMonitorConfigurator.cs` | DI configurator |
| `DepthMonitorFragment.cs` | Entity panel UI fragment |
| `DepthMonitorMarker.cs` | Marker component for monitored tiles |
| `DepthMonitorSpec.cs` | ComponentSpec record |

## Version Folders
- `Version-1.0` — targets game 1.0.x.x
- `Version-1.1` — targets game 1.1.x.x

## Hard Rule
DO NOT EVER TOUCH THE DEPLOY FOLDER.

BUILD DOES EVERYTHING, NEVER EVER MESS WITH THE DEPLOY PROCESS.
