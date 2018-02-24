using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {


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
            Debug.Log("Enter Valid Participant ID");
            return;
        }
        if (LoggerCSV.GetInstance().gameMode == LoggerCSV.NORMAL_MODE){
            SceneManager.LoadScene("familiarization");
        }
        else
            Debug.Log("training not set up yet");
    }
}
