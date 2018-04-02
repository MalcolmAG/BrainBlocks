using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainUIController : MonoBehaviour {

    public static int score;

    public TextMeshProUGUI scoreText;
    public Button pauseButton;
    public GameObject epoc;

	public GameObject finishedMessage;
    public GameObject midwayMessage;
    public GameObject pauseMessage;

    private bool checkingTime;
    private bool midWayReached;
    public float allotedTime;
    public float halfAllottedTime;

    public static bool paused = false;

//------------------------------Unity & Main Scene Control Functions------------------------------//


	void Start () {
        paused = false;
        halfAllottedTime = allotedTime / 2;
        midWayReached = false;
        checkingTime = true;
        score = 0;
        UI_Game();
	}
	
	void Update () {
        if(checkingTime)
            CheckTime();
        UpdateScore();
		
	}

    //Check if halfway or end has been reached
    void CheckTime(){
        halfAllottedTime -= Time.deltaTime;
        if(halfAllottedTime<0){
            paused = true;
            checkingTime = false;
            if(!midWayReached){
                LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_PAUSE_START);
				midWayReached = true;
                halfAllottedTime = allotedTime / 2;
                UI_Pause("midway");
            }
            else{
                LoggerCSV logger = LoggerCSV.GetInstance();
                if (logger.gameMode == LoggerCSV.BCI_MODE)
                    LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_END_BCI);
                else
                    logger.AddEvent(LoggerCSV.EVENT_END_NORMAL);
                UI_Pause("finished");
                logger.inSession = false;
                logger.SaveCSV();
                logger.ResetCSV();
            }
        }
    }

//------------------------------UI OnClick Functions------------------------------//


	//Called by Done_Button
	public void FinishGame(){
        if (LoggerCSV.GetInstance().gameMode == LoggerCSV.BCI_MODE)
		{
			GameObject master = GameObject.Find("Persistent_Master");
			Destroy(master.GetComponent<EmotivControl>());
			Destroy(master.GetComponent<EmoFacialExpression>());
		}
        SceneManager.LoadScene(0);
    }

	//Called by End_Midway_Button
	public void EndMidwayMessage(){
        LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_PAUSE_END);
        UI_Game();
        checkingTime = true;
        paused = false;
    }

	//Called by Pause_Button
	public void StartPause(){
        LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_PAUSE_START);
		checkingTime = false;
        paused = true;
        UI_Pause("pause");
	}

	//Called by End_Pause_Button
	public void EndPause(){
		LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_PAUSE_END);
		checkingTime = true;
        paused = false;
        UI_Game();
	}

//------------------------------UI Helper Functions------------------------------//

    //Sets UI elements for in-game view
	private void UI_Game()
	{
		if (LoggerCSV.GetInstance().gameMode == LoggerCSV.BCI_MODE)
			epoc.SetActive(true);
		midwayMessage.SetActive(false);
		finishedMessage.SetActive(false);
		pauseMessage.SetActive(false);
		pauseButton.gameObject.SetActive(true);
		scoreText.gameObject.SetActive(true);
	}

	//Sets UI elements for paused-game view
    private void UI_Pause(string type)
	{
		scoreText.gameObject.SetActive(false);
		pauseButton.gameObject.SetActive(false);
		switch (type)
		{
			case "midway":
				midwayMessage.SetActive(true);
				return;
			case "pause":
				pauseMessage.SetActive(true);
				return;
			case "finished":
				if (LoggerCSV.GetInstance().gameMode == LoggerCSV.BCI_MODE)
					epoc.SetActive(false);
				finishedMessage.SetActive(true);
				return;
		}
	}

	//Updates score UI element
	public void UpdateScore()
	{
		scoreText.text = "Score: " + score.ToString();
	}
}
