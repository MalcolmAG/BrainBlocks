using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Admin : MonoBehaviour {

    public Button btnAdmin;
    public TMP_InputField ifAdmin;

    private string password = "missChib";

    public void CheckPassword(string s){
        if(s == password){
            LoggerCSV log = LoggerCSV.GetInstance();
            log.AddEvent(LoggerCSV.EVENT_UNABLE);
            if (log.gameMode == LoggerCSV.BCI_MODE){
                log.AddEvent(LoggerCSV.EVENT_END_BCI);
            }
            else{
                log.AddEvent(LoggerCSV.EVENT_END_NORMAL);
            }
            log.SaveCSV();
            SceneManager.LoadScene(0);
        }
        else{
            btnAdmin.gameObject.SetActive(true);
            ifAdmin.text = string.Empty;
            ifAdmin.gameObject.SetActive(false);

        }

    }
}
