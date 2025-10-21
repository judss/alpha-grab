namespace AlphaGrabApp;

public static class TextExtractor
{
	public static async System.Threading.Tasks.Task<string> ExtractTextFromScreenshotAsync(string tempScreenshotPath)
	{
		if (string.IsNullOrWhiteSpace(tempScreenshotPath))
			throw new System.ArgumentException("tempScreenshotPath must be provided", nameof(tempScreenshotPath));

		if (!System.IO.File.Exists(tempScreenshotPath))
			throw new System.IO.FileNotFoundException("Screenshot file not found", tempScreenshotPath);

		var bitmap = await System.Threading.Tasks.Task.Run(() =>
		{
			try
			{
				return new Avalonia.Media.Imaging.Bitmap(tempScreenshotPath);
			}
			catch
			{
				return null;
			}
		});

		if (bitmap == null)
		{
			return string.Empty;
		}

		// For now return temp screenshot path — OCR will be added later.
		return tempScreenshotPath;
	}
}
