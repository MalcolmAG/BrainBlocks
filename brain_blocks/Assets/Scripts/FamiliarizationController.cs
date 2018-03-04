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

	public GameObject[] options;
    private GameObject group;
    private GameObject target;
    private float[] rotationOptions = { 0f, -90f, -180f, 90f };

    private int trialStage;

    public static bool paused = true;

    public void CustomStart(){
        instructionsMessage.gameObject.SetActive(false);
        if (LoggerCSV.GetInstance().gameMode == LoggerCSV.BCI_MODE)
			retrainButton.gameObject.SetActive(true);
        trialText.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(true);
		trialStage = 0;
        paused = false;
        FamiliarizationSet.runningTimer = Time.time;
		CreateNext();
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

        //Choose random pos and rotation
        int i = Random.Range(0, options.Length);
        Vector2 targetPos = new Vector2(Random.Range(0, 9), 0);
        Quaternion targetRot = new Quaternion(0, 0, Random.Range(0, rotationOptions.Length), 0);

        //Create trial objects
        target = Instantiate(options[i], targetPos, targetRot);
        SnapTarget(); //For when the random rotation/postioning put it out of bounds
        group = Instantiate(options[i], transform.position, Quaternion.identity);
        group.AddComponent<FamiliarizationSet>();
	}

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

    public bool CorrectOrientation(){
		//conditions are more complex for s and z groups
        float angle = Quaternion.Angle(target.transform.rotation, group.transform.rotation);
        //deal with UI element
        if(group.CompareTag("s") || group.CompareTag("z")){
            return (angle == 0 || angle == 180);
        }
        return angle == 0;
    }

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

    //Checks if user is done with trial
    void CheckStage(){
        if (trialStage == 5)
        {
            trialText.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(false);
            finishedMessage.SetActive(true);
			if (LoggerCSV.GetInstance().gameMode == LoggerCSV.BCI_MODE)
				retrainButton.gameObject.SetActive(false);
        }
        else
        {
            trialText.text = "Trial " + (++trialStage) + " of 5";
        }
    }

    public void StartPause(){
        paused = true;
		if(LoggerCSV.GetInstance().gameMode == LoggerCSV.BCI_MODE)
			retrainButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
        trialText.gameObject.SetActive(false);
        pausedMessage.SetActive(true);
    }

    public void EndPause(){
        paused = false;
        pausedMessage.SetActive(false);
        trialText.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
		if (LoggerCSV.GetInstance().gameMode == LoggerCSV.BCI_MODE)
			retrainButton.gameObject.SetActive(true);
    }

    public void NextScene(){
        SceneManager.LoadScene(3);
    }
    public void PreviousScene(){
        SceneManager.LoadScene(1);
    }
}
