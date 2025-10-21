using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace AlphaGrabApp;

public static class Screenshotter
{
	public static string? CaptureInteractiveToTempFile()
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			throw new PlatformNotSupportedException("MacScreenshot.CaptureInteractiveToTempFile is only supported on macOS.");

		var tempDir = Path.GetTempPath();
		var fileName = $"alphagrab_screenshot_{Guid.NewGuid():N}.png";
		var tempPath = Path.Combine(tempDir, fileName);

		var args = $"-i -x \"{tempPath}\"";

		try
		{
			using var proc = new Process();
			proc.StartInfo.FileName = "/usr/sbin/screencapture";
			proc.StartInfo.Arguments = args;
			proc.StartInfo.UseShellExecute = false;
			proc.StartInfo.CreateNoWindow = true;
			proc.StartInfo.RedirectStandardOutput = false;
			proc.StartInfo.RedirectStandardError = true;

			proc.Start();

			var exited = proc.WaitForExit(120_000);
			if (!exited)
			{
				try { proc.Kill(); } catch { }
				return null;
			}

			if (proc.ExitCode != 0)
			{
				if (File.Exists(tempPath))
				{
					try { File.Delete(tempPath); } catch { }
				}
				return null;
			}

			if (!File.Exists(tempPath))
				return null;

			var info = new FileInfo(tempPath);
			if (info.Length == 0)
			{
				try { File.Delete(tempPath); } catch { }
				return null;
			}

			return tempPath;
		}
		catch (Exception)
		{
			try { if (File.Exists(tempPath)) File.Delete(tempPath); } catch { }
			return null;
		}
	}

	public static async System.Threading.Tasks.Task<bool> DeleteTempScreenshotAsync(string? tempScreenshotPath)
	{
		if (string.IsNullOrWhiteSpace(tempScreenshotPath))
			return true;

		return await System.Threading.Tasks.Task.Run(() =>
		{
			try
			{
				if (File.Exists(tempScreenshotPath))
				{
                    File.Delete(tempScreenshotPath);
				}
				return true;
			}
			catch
			{
				return false;
			}
		});
	}
}