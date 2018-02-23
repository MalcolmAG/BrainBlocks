using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MasterController : MonoBehaviour {

	public static readonly int NORMAL_MODE = 0;
	public static readonly int BCI_MODE = 1;

    public static int gameMode = 0;


    void Start(){
        DontDestroyOnLoad(gameObject);
        gameMode = NORMAL_MODE;
    }

    //Activated when slider in menu is used
	public void SetGameMode(float val)
	{
		gameMode = (int)val;
	}

    //Activated by start button in menu
    public void StartGame(){
        if (gameMode == NORMAL_MODE){
            SceneManager.LoadScene("familiarization");
        }
        else
            Debug.Log("training not set up yet");
    }
}
