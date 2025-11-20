// This script is supposed to be used with the Trigger functionality in DisplayFusion to attach
// to a specific window. By default, this is set to notepad.exe
//
// For a feature which can be toggled onto any window, take a look in FunctionVersion.cs

using System;
using System.Drawing;

public static class DisplayFusionFunction
{
    // Which exe will be auto-size-back'd
    private const string targetExe = "notepad.exe";
    // PLEASE CHANGE notepad.exe ABOVE TO THE EXE YOU WANT TO TRIGGER ANTI-RESIZE ON

    public static void Run()
    {
        // Get the currently focused window
        IntPtr _window = BFS.Window.GetFocusedWindow();
        if (_window == IntPtr.Zero)
        {
            return;
        }

        // Get the main file (full path) for the process that owns this window
        string fullPath = BFS.Application.GetMainFileByWindow(_window); // returns full path or null
        if (String.IsNullOrEmpty(fullPath))
        {
            return;
        }

        // Extract filename from path and compare case-insensitively
        string fileName = System.IO.Path.GetFileName(fullPath).ToLowerInvariant();
        if (!fileName.Equals(targetExe.ToLowerInvariant()))
        {
            return;
        }

        // Save the current bounds
        Rectangle originalBounds = BFS.Window.GetBounds(_window);

        // Loop while the window is still valid/visible and owned by same exe
        while (_window != IntPtr.Zero && BFS.Window.IsVisible(_window))
        {
            // If the window was closed, break
            string currentPath = BFS.Application.GetMainFileByWindow(_window);
            if (String.IsNullOrEmpty(currentPath) 
                || !System.IO.Path.GetFileName(currentPath).Equals(targetExe, StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

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
    }
}