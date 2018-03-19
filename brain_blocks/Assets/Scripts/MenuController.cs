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
	
    //Coroutine function to display message to user
    IEnumerator ShowMessage(string message, float delay)
	{
        messageText.text = message;
        messagePanel.SetActive(true);
		yield return new WaitForSeconds(delay);
        messagePanel.SetActive(false);
	}

	//------------------------------UI OnClick Functions------------------------------//

	//Called by Game_Mode_Slider
	public void SetGameMode(float val)
	{
        LoggerCSV.GetInstance().gameMode = (int)val;

	}
    //Called by Participant_ID_InputField
    public void SetParticipantID(string val){
        int id;
        Int32.TryParse(val, out id);
        LoggerCSV.GetInstance().participantID = id;
    }

    //Called by Start_Button
    public void StartGame(){
        if (LoggerCSV.GetInstance().participantID < 1){
            StartCoroutine(ShowMessage("Please Enter a Valid ID", 1.5f));
            return;
        }
        if (LoggerCSV.GetInstance().gameMode == LoggerCSV.NORMAL_MODE){
            LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_START_NORMAL);
            SceneManager.LoadScene(2);
        }
        else{
            GameObject master = GameObject.Find("Persistent_Master");
            master.AddComponent<EmotivControl>();
            master.AddComponent<EmoFacialExpression>();
            LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_START_BCI);
            SceneManager.LoadScene(1);
        }
    }
}