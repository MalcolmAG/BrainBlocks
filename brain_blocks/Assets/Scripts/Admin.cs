using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
//Controls the Admin button and input field in Trail/Familiarization stage
//Researcher enters in password to terminate session prior to completion
public class Admin : MonoBehaviour {

    public Button btnAdmin;
    public TMP_InputField ifAdmin;

    private string password = "missChib";

    public void CheckPassword(string s){
        if(s == password){
            LoggerCSV logger = LoggerCSV.GetInstance();
            logger.AddEvent(LoggerCSV.EVENT_UNABLE);
            if (logger.gameMode == LoggerCSV.BCI_MODE){
                logger.AddEvent(LoggerCSV.EVENT_END_BCI);
            }
            else{
                logger.AddEvent(LoggerCSV.EVENT_END_NORMAL);
            }
            logger.inSession = false;
            logger.SaveCSV();
            logger.ResetCSV();

            SceneManager.LoadScene(0);
        }
        else{
            btnAdmin.gameObject.SetActive(true);
            ifAdmin.text = string.Empty;
            ifAdmin.gameObject.SetActive(false);

        }

    }
}
