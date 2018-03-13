using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FamiliarizationController : MonoBehaviour {

    public TextMeshProUGUI trialText;

    public Button retrainButton;
    public Button pauseButton;
    public GameObject instructionsMessage;
    public GameObject finishedMessage;
    public GameObject pausedMessage;
    public GameObject epoc;

	public GameObject[] options;
    public int maxStage=5;
    private GameObject group;
    private GameObject target;
    private float[] rotationOptions = { 0f, -90f, -180f, 90f };

    private int trialStage;
    private float startTime;

    public static bool paused = true;

//------------------------------Familiarization Scene Control Functions------------------------------//

	//Spawns "preview" group at top of game area
	//Randmomly chooses next "preview" group
	public void CreateNext()
	{
        //Check if familiarization stage is finished
        CheckStage();
        //Destroy objects from previous trial
        Destroy(group);
        Destroy(target);

        //Choose random position and rotation of prompt
        int i = Random.Range(0, options.Length);
        Vector2 targetPos = new Vector2(Random.Range(0, 9), 0);
        Quaternion targetRot = new Quaternion(0, 0, Random.Range(0, rotationOptions.Length), 0);

        //Create trial objects
        target = Instantiate(options[i], targetPos, targetRot);
        SnapTarget(); //For when the random rotation/postioning put it out of bounds
        group = Instantiate(options[i], transform.position, Quaternion.identity);
        group.AddComponent<FamiliarizationSet>();
	}

    //Snaps target so it is within game area
    private void SnapTarget(){
        int below = 0; //lowest possible grid pos
        int left = 0; //leftmost possible grid pos
        int right = 9; //rightmost possible grid pos
        //Check if out of bounds
        foreach(Transform child in target.transform){
            Vector2 v = Grid.ToGrid(child.position);
            if (v.y < 0 && v.y < below)
                below = (int)v.y;
            if (v.x < 0 && v.x < left)
                left = (int)v.x;
            if (v.x > 9 && v.x > right)
                right = (int)v.x;
        }
        //Snap into bounds
        target.transform.Translate(Vector3.up * below);
        target.transform.Translate(Vector3.right * left);
        target.transform.Translate(Vector3.left * (right - 9));
    }

    //Compares orientation of player's block to target
    public bool CorrectOrientation(){
		//conditions are more complex for s and z groups
        float angle = Quaternion.Angle(target.transform.rotation, group.transform.rotation);
        //deal with UI element
        if(group.CompareTag("s") || group.CompareTag("z")){
            return (angle == 0 || angle == 180);
        }
        return angle == 0;
    }

	//Compares position of player's block to target
	public bool CorrectPosition(){
        float t = 0;
        float g = 0;
		//Must find and compare avg block position
		//because parent locations may not add up 
		//with groups s and z
		foreach(Transform child in target.transform){
            t += child.position.x;
        }
        foreach (Transform child in group.transform)
		{
			g += child.position.x;
		}
        //deal with UI element

        return (t/4) == (g/4);
    }

    //Checks if user is done with familiarization trials
    void CheckStage(){
        if (trialStage == maxStage)
        {
            LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_FAMI_END);
            ToggleUI(true, "finished");
        }
        else
            trialText.text = "Trial " + (++trialStage) + " of 5";
    }

//------------------------------UI OnClick Functions------------------------------//
    //Called by Start_Trials_Buttom
	public void CustomStart()
	{
        LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_FAMI_START);
		InitUI();
		startTime = Time.time;
		trialStage = 0;
		paused = false;
		ToggleUI(paused, "none");
		FamiliarizationSet.runningTimer = Time.time;
		CreateNext();
	}

    //Called by Pause_Button
    public void StartPause(){
        LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_PAUSE_START);
        paused = true;
        ToggleUI(paused, "pause");
    }

	//Called by End_Pause_Button
	public void EndPause(){
        LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_PAUSE_END);
		paused = false;
        ToggleUI(paused, "pause");
    }

	//Called by Next_Scene_Button and Retrain_Button
	public void LoadScene(int idx){
        SceneManager.LoadScene(idx);
    }

//------------------------------UI Helper Functions------------------------------//

    //Set Up UI Objects after instructions are read
    private void InitUI(){
        instructionsMessage.gameObject.SetActive(false);
		if (LoggerCSV.GetInstance().gameMode == LoggerCSV.BCI_MODE)
		{
            epoc.SetActive(true);
		}
    }

    //Modifies UI element view
    private void ToggleUI(bool pause, string type){
		if (LoggerCSV.GetInstance().gameMode == LoggerCSV.BCI_MODE)
		{
            retrainButton.gameObject.SetActive(!pause);
		}
		pauseButton.gameObject.SetActive(!paused);
		trialText.gameObject.SetActive(!pause);
        switch(type){
            case "pause":
                pausedMessage.SetActive(pause);
                return;
            case "finished":
                finishedMessage.SetActive(pause);
                return;
            default:
                return;
        }
    }
}
