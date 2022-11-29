using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    private const string path = "";

    public static class KeyBinding
    {
        public static readonly KeyCode[] DisableKeycodes =
        {
            KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2, KeyCode.Mouse3,
            KeyCode.Mouse4, KeyCode.Mouse5, KeyCode.Mouse6, KeyCode.Backspace,
            KeyCode.Return, KeyCode.LeftShift, KeyCode.RightShift, KeyCode.Delete,
            KeyCode.PageUp, KeyCode.PageDown, KeyCode.Home, KeyCode.End,
            KeyCode.Escape, KeyCode.Print, KeyCode.Insert, KeyCode.LeftWindows,
            KeyCode.RightWindows, KeyCode.CapsLock, KeyCode.ScrollLock, KeyCode.Tab,
            KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow,
            KeyCode.LeftControl, KeyCode.RightControl, KeyCode.LeftAlt, KeyCode.RightAlt,
            KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5, KeyCode.F6,
            KeyCode.F7, KeyCode.F8, KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12,
            KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
            KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,
            KeyCode.Minus, KeyCode.Equals, KeyCode.BackQuote, KeyCode.AltGr, KeyCode.KeypadEnter
        };
        public static KeyCode[] UserKeyCodes = new KeyCode[11];

        public static bool IsKeyCodeAvailable(KeyCode _kc)
        {
            if (DisableKeycodes.Contains(_kc)) { return false; }
            else { return true; }
        }
        public static void GetDataFromPlayMode()
        {

        }
        public static void SetDataToPlayMode()
        {
            
        }
    }
    public static class PlayOption
    {

    }

    public static void SaveData()
    {

    }
    public static void LoadData()
    {

    }
}

public class SaveUserData
{
    public int[] SaveUserKeyCode = new int[12];
}
