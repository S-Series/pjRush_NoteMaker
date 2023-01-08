using UnityEngine;
using System.Collections;


public class ShowFps : MonoBehaviour
{
    float deltaTime = 0.0f;

    static GUIStyle style;
    static Rect rect;
    static float msec;
    float fps;
    string text;

    void Awake()
    {
        int w = Screen.width, h = Screen.height;

        rect = new Rect(0, h - (h * 2 / 100), w, h * 2 / 100);

        style = new GUIStyle();
        style.alignment = TextAnchor.LowerLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.white;
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()//소스로 GUI 표시.
    {
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;  //초당 프레임 - 1초에

        text = " " + fps.ToString("F1") + "fps (" + msec.ToString("F1") + "ms" + ")";
        GUI.Label(rect, text, style);
    }
    public static void UpdateResolution()
    {
        int w = Screen.width, h = Screen.height;

        rect = new Rect(0, h - (h * 2 / 100), w, h * 2 / 100);

        style = new GUIStyle();
        style.alignment = TextAnchor.LowerLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.white;
    }
}
