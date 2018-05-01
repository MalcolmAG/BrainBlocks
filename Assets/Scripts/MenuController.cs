using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Controls Menu
/// </summary>
public class MenuController : MonoBehaviour {

    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    private readonly int NORMAL = 0;
    private int mode;

    private void Start()
    {
        mode = NORMAL;

	}

    public void SetMode(float m){
        mode = (int) m;
    }

    //------------------------------UI OnClick Functions------------------------------//

    /// <summary>
    /// Starts the game. Called by Start_Button
    /// </summary>
    public void StartGame(){
        //Start saving data automatically
        if (mode == NORMAL){
            SceneManager.LoadScene(3);
        }
        else{
            GameObject master = GameObject.Find("Persistent_Master");
            master.AddComponent<EmotivControl>();
            master.AddComponent<EmoFacialExpression>();
            SceneManager.LoadScene(1);
        }
    }

	/// <summary>
	/// Quit the game. Called by Quit_Button
	/// </summary>
	public void Quit()
	{
		Application.Quit();
	}
}