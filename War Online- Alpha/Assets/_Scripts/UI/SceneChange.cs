using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour {
    public SceneChange Scene;
    public Button btn;
    public string scenename;
	// Use this for initialization
	void Start () {
        Button btnclick = btn.GetComponent<Button>();
        btnclick.onClick.AddListener(TaskOnClick);
	}
	
	void TaskOnClick()
    {
        SceneManager.LoadScene(scenename);
    }
}
