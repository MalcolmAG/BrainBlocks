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
	private float  startingTime;
    public float allotedTime;
    public float halfAllottedTime;

    public static bool paused = false;
    private float timeOffset;
    private Set pausedSet;

//------------------------------Unity & Main Scene Control Functions------------------------------//


	void Start () {
        paused = false;
        halfAllottedTime = allotedTime / 2;
        midWayReached = false;
        checkingTime = true;
        startingTime = Time.time; 
        score = 0;
        UI_Game();
	}
	
	void Update () {
        if(checkingTime)
            CheckTime();
        if(Input.GetKeyDown(KeyCode.X)){
            Debug.Log(score);
        }
        UpdateScore();
		
	}

    //Check if halfway or end has been reached
    void CheckTime(){
        halfAllottedTime -= Time.deltaTime;
        if(halfAllottedTime<0){
            paused = true;
            checkingTime = false;
            if(!midWayReached){
				midWayReached = true;
                halfAllottedTime = allotedTime / 2;
                UI_Pause("midway");
            }
            else{
                UI_Pause("finished");
				LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_SCORE, score);
                LoggerCSV.GetInstance().SaveCSV();
            }
        }
    }

//------------------------------UI OnClick Functions------------------------------//


	//Called by Done_Button
	public void FinishGame(){
        LoggerCSV logger = LoggerCSV.GetInstance();
        logger.gameMode = LoggerCSV.NORMAL_MODE;
        logger.participantID = -1;
        SceneManager.LoadScene(0);
    }

	//Called by End_Midway_Button
	public void EndMidwayMessage(){
        UI_Game();
        startingTime = Time.time;
        checkingTime = true;
        paused = false;
    }

	//Called by Pause_Button
	public void StartPause(){
        checkingTime = false;
        pausedSet = FindObjectOfType<Set>();
        timeOffset = Time.time - pausedSet.runningTimer;
        paused = true;
        UI_Pause("pause");
	}

	//Called by End_Pause_Button
	public void EndPause(){
        checkingTime = true;
        pausedSet.runningTimer = Time.time - timeOffset;
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
