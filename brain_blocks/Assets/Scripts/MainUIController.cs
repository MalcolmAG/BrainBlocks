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

	// Use this for initialization
	void Start () {
        paused = false;
        halfAllottedTime = allotedTime / 2;
        if (LoggerCSV.GetInstance().gameMode == LoggerCSV.BCI_MODE)
            epoc.SetActive(true);
        midwayMessage.SetActive(false);
        finishedMessage.SetActive(false);
        midWayReached = false;
        checkingTime = true;
        startingTime = Time.time; 
        score = 0;
	}
	
	// Update is called once per frame
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
            scoreText.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(false);
            checkingTime = false;
            if(!midWayReached){
				midWayReached = true;
                midwayMessage.SetActive(true);
                halfAllottedTime = allotedTime / 2;
            }
            else{
				if (LoggerCSV.GetInstance().gameMode == LoggerCSV.BCI_MODE)
					epoc.SetActive(false);
                finishedMessage.SetActive(true);
				LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_SCORE, score);
                LoggerCSV.GetInstance().SaveCSV();
            }
        }
    }

    //Updates score UI element
    public void UpdateScore(){
        scoreText.text = "Score: " + score.ToString();
    }

    //OnClick for Done_Button
    public void FinishGame(){
        LoggerCSV logger = LoggerCSV.GetInstance();
        logger.gameMode = LoggerCSV.NORMAL_MODE;
        logger.participantID = -1;
        SceneManager.LoadScene(0);
    }

    //OnClick for End_Midway_Button
    public void EndMidwayMessage(){
		scoreText.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(true);
        midwayMessage.SetActive(false);
        startingTime = Time.time;
        checkingTime = true;
        paused = false;
    }

    //OnClick for Pause_Button
    public void StartPause(){
        checkingTime = false;
        pausedSet = FindObjectOfType<Set>();
        timeOffset = Time.time - pausedSet.runningTimer;
        paused = true;
        pauseMessage.SetActive(true);
		scoreText.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
	}

    //OnClick for End_Pause_Button
    public void EndPause(){
        checkingTime = true;
        pausedSet.runningTimer = Time.time - timeOffset;
        paused = false;
		pauseMessage.SetActive(false);
		scoreText.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(true);
	}
}
