using UnityEngine;
using System.Collections;
 
public class FPSDisplayer : MonoBehaviour
{
	float deltaTime = 0.0f;
	public Camera[] cams;

    private void Start()
    {
		//Screen.SetResolution(800, 450, true);
		foreach(Camera cam in cams)
        {
			//cam.aspect = 16f / 9f;
        }
    }

    void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}
 
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
 
		GUIStyle style = new GUIStyle();
 
		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}

}
