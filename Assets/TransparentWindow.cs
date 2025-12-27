using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TransparentWindow : MonoBehaviour
{
    //Calls windows functions
    [DllImport("user32.dll")]
    //Returns currently active window by its handle
    static extern IntPtr GetActiveWindow();
    
    [DllImport("user32.dll")]
    //Modifies windows flags, controlling how it behaves
    //hwnd -> Which window to modify
    //nIndex -> which property to change
    //dwNewLong -> new value
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    
    [DllImport("user32.dll")]
    //Controls how transparency works for layered windows
    //hwnd -> The window
    //crKey -> color key
    //bAlpha -> opacity
    //dwFlags -> which mode to use
    static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    //Lets windows know user wants to change the extended windows style.
    const int GWL_EXSTYLE = -20;
    
    //Enables layered window rendering, required for transparency.
    const uint WS_EX_LAYERED = 0x80000;
    
    //Makes the window click through, mouse events pass to whatever is behind the window
    const uint WS_EX_TRANSPARENT = 0x20;
    
    //Tells windows to use alpha blending not a color key
    const uint LWA_ALPHA = 0x2;

    void Start()
    {
        //Gets the unity window handle
        IntPtr hwnd = GetActiveWindow();

        //Changes windows extended style, enables layered rendering
        SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED);
        
        //Applies transparency
        SetLayeredWindowAttributes(hwnd, 0, 255, LWA_ALPHA);
    }
}
