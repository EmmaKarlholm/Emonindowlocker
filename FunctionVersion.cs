// This script is supposed to be used with keyboard shortcuts or titlebar integrations
// in DisplayFusion to attach any window.
//
// For a feature which can instead be set in Triggers to automatically fire every time
// a specific application is launched, take a look in TriggerVersion.cs

using System;
using System.Drawing;

public static class DisplayFusionFunction
{
	private static IntPtr lockedWindow = IntPtr.Zero;
	private static Rectangle originalBounds;

	public static void Run()
	{
		// Get the currently focused window
		IntPtr _window = BFS.Window.GetFocusedWindow();
		if (_window == IntPtr.Zero)
		{
			return;
		}

		// Unlock if it's already locked
		if (_window == lockedWindow)
		{
			lockedWindow = IntPtr.Zero;
			BFS.Dialog.ShowTrayMessage("Window Unlocked");
			return;
		}

		// Lock it since it wasn't locked
		lockedWindow = _window;
		originalBounds = BFS.Window.GetBounds(_window);
		BFS.Dialog.ShowTrayMessage("Window Locked");


		// Loop while the window is still valid/visible
		while (lockedWindow == _window && BFS.Window.IsVisible(_window))
		{
			Rectangle currentBounds = BFS.Window.GetBounds(_window);

			// Reize back to original size if height/width changed
			if (currentBounds.Width != originalBounds.Width
				|| currentBounds.Height != originalBounds.Height)
			{
				// Keep current X/Y so movement is allowed, but force size
				BFS.Window.SetSize(_window, originalBounds.Width, originalBounds.Height);
			}

			// Small wait to reduce CPU usage
			BFS.General.ThreadWait(120);
		}

		lockedWindow = IntPtr.Zero;
	}
}