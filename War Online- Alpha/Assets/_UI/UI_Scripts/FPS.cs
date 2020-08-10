using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FPS : MonoBehaviour
{
	float deltaTime = 0.0f;
	[SerializeField] float xPos;
	[SerializeField] float yPos;
	[SerializeField] Color color;

	void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(xPos, yPos, w, h * 4 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 4 / 100;
		style.normal.textColor = color;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
}
