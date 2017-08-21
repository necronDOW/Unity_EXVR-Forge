using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public int fontSize = 24;

    private float deltaTime = 0.0f;
    private Rect rect;
    private GUIStyle style;

    private void Start()
    {
        ConfigureGUIStyle();
    }

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    private void ConfigureGUIStyle()
    {
        int w = Screen.width, h = Screen.height;
        rect = new Rect(0, 0, w, h * 2 / 100);

        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = fontSize;
        style.normal.textColor = new Color(.0f, .0f, .5f, 1f);
    }

    private void OnGUI()
    {
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.} fps", fps);
        GUI.Label(rect, text, style);
    }
}
