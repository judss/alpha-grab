# CLAUDE.md

This file provides guidance to Claude when working with code in this repository.

## Project Overview

AlphaGrab is a macOS system tray application built with Avalonia UI (.NET 9.0) that enables interactive text extraction from screenshots. The app runs as a background service with a tray icon, allowing users to capture a screen region and see the extracted text in a popup. OCR functionality is currently a placeholder.

## Architecture

### Core Components

- **Program.cs** — Entry point and Avalonia app builder configuration
- **App.axaml.cs** — Main application lifecycle: tray icon setup, menu handlers, screenshot workflow orchestration
- **Screenshotter.cs** — macOS-specific screenshot capture via `/usr/sbin/screencapture -i -x`
- **TextExtractor.cs** — OCR placeholder; currently returns the screenshot file path instead of extracted text
- **Toast.cs** — Result popup window (500×200, bottom-right, acrylic blue-violet); click expands to an 800×200 editable text view
- **MainWindow.axaml.cs** — Minimal window component, not shown by default

### Key Design Patterns

- System tray app with `ShutdownMode.OnExplicitShutdown` — no visible window at startup
- Async/await throughout; UI updates marshaled via `Dispatcher.UIThread.InvokeAsync()`
- Platform guard: `RuntimeInformation.IsOSPlatform(OSPlatform.OSX)` — throws `PlatformNotSupportedException` on non-macOS
- Temp files use GUID-based names (`alphagrab_screenshot_{guid}.png`) and are cleaned up after extraction

### Screenshot Workflow

1. User clicks "Grab Text" in the tray menu
2. `Screenshotter.CaptureInteractiveToTempFile()` spawns `screencapture -i -x` (interactive, silent)
3. Screenshot saved to system temp directory; 120-second timeout with graceful process kill
4. `TextExtractor.ExtractTextFromScreenshotAsync()` processes the image (currently returns file path)
5. `Toast` window displays the result
6. `DeleteTempScreenshotAsync()` cleans up the temp file

## Development Commands

```bash
# Build
dotnet build src/AlphaGrab.App/AlphaGrabApp.csproj

# Run
dotnet run --project src/AlphaGrab.App/AlphaGrabApp.csproj

# Watch mode (hot reload)
dotnet watch run --project src/AlphaGrab.App/AlphaGrabApp.csproj

# Publish
dotnet publish src/AlphaGrab.App/AlphaGrabApp.csproj
```

## Platform Requirements

- **macOS only** for screenshot capture (uses `/usr/sbin/screencapture`)
- Screen Recording permission required (macOS prompts on first use)
- .NET 9.0 SDK

## Dependencies

| Package | Version | Purpose |
|---|---|---|
| Avalonia | 11.3.6 | Cross-platform UI framework |
| Avalonia.Desktop | 11.3.6 | Desktop integration (tray icon) |
| Avalonia.Themes.Fluent | 11.3.6 | UI theme |
| Avalonia.Fonts.Inter | 11.3.6 | Typography |
| Avalonia.Diagnostics | 11.3.6 | Debug tooling (excluded in Release) |
| ReactiveUI | 22.1.1 | Reactive programming support |

## Current Status

**OCR is not yet implemented.** `TextExtractor.cs` is a placeholder that returns the screenshot file path. The next major milestone is integrating a real OCR engine (e.g., Tesseract, Apple Vision framework, Azure Cognitive Services).

## Future Development

When implementing OCR:
- Replace the placeholder in `TextExtractor.ExtractTextFromScreenshotAsync()`
- Add image preprocessing if needed (contrast, scale) before passing to OCR
- Handle OCR errors gracefully and surface them in the Toast UI
- Consider adding a "Copy to Clipboard" action in the Toast window (infrastructure already exists)

## Testing Considerations

- Mock `screencapture` calls for platform-independent unit tests
- Test async file operations and temp file cleanup
- Cover the 120-second timeout/kill path in `Screenshotter`
- Verify tray icon menu items wire up correctly
