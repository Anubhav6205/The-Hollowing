using UnityEngine;
using System.Collections;

/// <summary>
/// Displays real-time frames per second (FPS) and frame time in milliseconds
/// using Unity's OnGUI method.
/// </summary>
public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;

    void Update()
    {
        // Smooths out deltaTime to avoid FPS fluctuation spikes
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width;
        int h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(1f, 1f, 1f, 1f); // white

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
