# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

AlphaGrabApp is a macOS system tray application built with Avalonia UI (.NET 9.0) that enables interactive text extraction from screenshots. The app runs as a background service with a tray icon and provides OCR functionality through the macOS screencapture utility.

## Architecture

### Core Components
- **Program.cs**: Entry point and Avalonia app configuration
- **App.axaml.cs**: Main application lifecycle with tray icon management and screenshot workflow
- **MacScreenshot.cs**: macOS-specific screenshot capture using `/usr/sbin/screencapture`
- **TextExtractor.cs**: Text extraction from screenshots (OCR placeholder - currently returns file path)
- **MainWindow.axaml.cs**: Window component (currently minimal)

### Key Design Patterns
- System tray application with no main window by default
- Async/await patterns for screenshot processing
- Platform-specific code with runtime checks (`RuntimeInformation.IsOSPlatform`)
- Temporary file management for screenshot processing

## Development Commands

### Build & Run
```bash
# Build the project
dotnet build AlphaGrabApp.csproj

# Run the application
dotnet run --project AlphaGrabApp.csproj

# Watch mode for development
dotnet watch run --project AlphaGrabApp.csproj

# Publish for distribution
dotnet publish AlphaGrabApp.csproj
```

### Debugging
- Use VS Code launch configuration `.NET Core Launch (console)` for debugging
- Pre-launch task automatically builds the project
- Debug target: `bin/Debug/net9.0/AlphaGrabApp.dll`

## Platform Requirements

### macOS Specific
- Uses `/usr/sbin/screencapture` for interactive screenshot capture
- Requires macOS permissions for screen recording
- Platform checks enforce macOS-only execution for screenshot functionality

### Dependencies
- **Avalonia UI 11.3.6**: Cross-platform UI framework
- **ReactiveUI 22.1.1**: Reactive programming framework
- **Inter Font**: Default typography
- **.NET 9.0**: Target framework with nullable reference types enabled

## Key Behaviors

### Screenshot Workflow
1. User clicks "Grab Text" from tray menu
2. Interactive screenshot capture launches (`screencapture -i -x`)
3. Screenshot saved to temp file with unique GUID
4. Text extraction attempted (currently placeholder)
5. Results displayed in popup window
6. Temp file cleanup

### Error Handling
- Timeout protection (120 seconds for screenshot capture)
- File existence validation
- Process exit code checking
- Graceful cleanup on failures

## Testing Considerations

When implementing tests:
- Mock platform-specific calls to `screencapture`
- Test async file operations and cleanup
- Verify tray icon menu functionality
- Test screenshot timeout scenarios

## Future Development

The TextExtractor.cs currently returns the file path as a placeholder. OCR implementation will need:
- Integration with OCR library (Tesseract, Azure Cognitive Services, etc.)
- Image preprocessing for better text recognition
- Error handling for OCR failures