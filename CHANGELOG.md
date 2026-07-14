# Changelog

## [Unreleased]

## [0.0.2]

### Added

- Compact list mode when the slider is at min zoom.
- The currently loaded scenes are now highlighted in the grid with an accent border, and the highlight follows you when hopping scenes.

### Changed

- Restyled the overlay to follow the Unity Editor theme: all hard-coded colors were replaced with editor theme variables, so the overlay now renders correctly in both the dark and light skins.
- Scene buttons got rounded corners, spacing between grid cells, and hover/pressed feedback.
- Moved inline styles from the UXML and C# into `SceneHop.uss`.


## [0.0.1]

### Added

- A UI overlay that allows switching between scenes via buttons. Supports fetching scenes by path, name, or listing all scenes in the project.
- `package.json` manifest.
- MIT license file.
