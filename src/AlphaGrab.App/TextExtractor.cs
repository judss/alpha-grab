using System;
using System.IO;
using System.Threading.Tasks;

namespace AlphaGrabApp;

public static class TextExtractor
{
    public static Task<string> ExtractTextFromScreenshotAsync(string tempScreenshotPath)
    {
        if (string.IsNullOrWhiteSpace(tempScreenshotPath))
            throw new ArgumentException("tempScreenshotPath must be provided", nameof(tempScreenshotPath));

        if (!File.Exists(tempScreenshotPath))
            throw new FileNotFoundException("Screenshot file not found", tempScreenshotPath);

        // OCR not yet implemented — returns dummy string
        return Task.FromResult("Testing OCR functionality. This is a placeholder text.");
    }
}
