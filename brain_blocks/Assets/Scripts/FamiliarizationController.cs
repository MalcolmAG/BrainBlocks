using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FamiliarizationController : MonoBehaviour {

    public TextMeshProUGUI trialText;

    public Button pauseButton;
    public GameObject instructionsMessage;
    public GameObject finishedMessage;
    public GameObject pausedMessage;
    public GameObject epoc;

	public GameObject timeOutPanel;

    public GameObject[] options;
    public int maxStage = 6;
    private bool leftFirst;
    private bool spawnLeft;
    private GameObject group;
    private GameObject target;
    private float[] rotationOptions = { 0f, -90f, -180f, 90f };
    private float timePerTrial = 120.25f;
    private float runningTimer;
    private int trialStage;
    private bool started = false;
    public static bool paused = true;

    //------------------------------Familiarization Scene Control Functions------------------------------//

    //For timing trials
    private void Update()
    {
        if (started && !paused)
        {
            runningTimer += Time.deltaTime;
            if (runningTimer > timePerTrial)
            {
                LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_FAMI_TIMEOUT);
                timeOutPanel.SetActive(true);
            }
                
        }
        
    }

    //Spawns "preview" group at top of game area
    //Randmomly chooses next "preview" group
    public void CreateNext()
	{
        //Check if familiarization stage is finished
        CheckStage();
        //Destroy objects from previous trial
        Destroy(group);
        Destroy(target);

        //Create Player Object
		int i = Random.Range(0, options.Length);
        group = Instantiate(options[i], transform.position, Quaternion.identity);

        //Choose random position and rotation of prompt
        //Loop ensures target is not directly below group
        while (true)
        {
            Vector2 targetPos;
            if (spawnLeft)
                targetPos = new Vector2(Random.Range(0, 4), 0);
            else
                targetPos = new Vector2(Random.Range(5, 9), 0);
                
            Quaternion targetRot = new Quaternion(0, 0, Random.Range(0, rotationOptions.Length), 0);
            //Create trial object
            target = Instantiate(options[i], targetPos, targetRot);
            //Check if directly below
            //Average needed because different rotations create different x vals
            if (PositionAverage(target.transform) != PositionAverage(group.transform)){
                spawnLeft = !spawnLeft;
                break;
            }
            Destroy(target);
            Debug.Log("rerandomizing");
        }
		SnapTarget(); //For when the random rotation/postioning put it out of bounds

		LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_FAMI_PROMT, 
                                         PositionAverage(target.transform).ToString());
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
        float t = PositionAverage(target.transform);
        float g = PositionAverage(group.transform);
        return (t/4) == (g/4);
    }

	//Returns average x position of transform
	//Used to compare prompt to player's block
	//Must find and compare avg block position
	//because parent locations may not add up 
	//with groups s and z
	public static float PositionAverage(Transform t){
        float x = 0;
        foreach(Transform child in t){
            x += child.position.x;
        }
        return x / 4f;
    }

    //Checks if user is done with familiarization trials
    void CheckStage(){
        if(trialStage != 0)
            LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_FAMI_PASS);
        if (trialStage == maxStage)
        {
            LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_FAMI_END);
            //Stop checking time
            started = false;
            ToggleUI(true, "finished");
        }
        else{
            runningTimer = 0f;
            trialText.text = "Trial " + (++trialStage) + " of " + maxStage;
        }
    }

//------------------------------UI OnClick Functions------------------------------//
    //Called by Start_Trials_Buttom
	public void CustomStart()
	{
        LoggerCSV logger = LoggerCSV.GetInstance();
        logger.AddEvent(LoggerCSV.EVENT_FAMI_START);
        leftFirst = logger.counterBalanceID == 1
                          || logger.counterBalanceID == 3;
        spawnLeft = leftFirst;
		InitUI();
		trialStage = 0;
		paused = false;
		ToggleUI(paused, "none");
        runningTimer = 0f;
        started = true;
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

	//Called by Next_Scene_Button
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
		pauseButton.gameObject.SetActive(!pause);
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
