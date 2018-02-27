using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
	

    IEnumerator ShowMessage(string message, float delay)
	{
        messageText.text = message;
        messagePanel.SetActive(true);
		yield return new WaitForSeconds(delay);
        messagePanel.SetActive(false);
	}

    //Activated when slider in menu is used
	public void SetGameMode(float val)
	{
        LoggerCSV.GetInstance().gameMode = (int)val;

	}
    //Activated when input val is changed
    public void SetParticipantID(string val){
        int id;
        Int32.TryParse(val, out id);
        LoggerCSV.GetInstance().participantID = id;
    }

    //Activated by start button in menu
    public void StartGame(){
        if (LoggerCSV.GetInstance().participantID < 1){
            StartCoroutine(ShowMessage("Please Enter a Valid ID", 1.5f));
            return;
        }
        if (LoggerCSV.GetInstance().gameMode == LoggerCSV.NORMAL_MODE){
            SceneManager.LoadScene(2);
        }
        else
            Debug.Log("training not set up yet");
    }
}
