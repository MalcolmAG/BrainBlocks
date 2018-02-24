using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FamiliarizationController : MonoBehaviour {

    public Button retrain;
    public GameObject finishedMessage;

	public GameObject[] options;
    private GameObject group;
    private GameObject target;
    private float[] rotationOptions = { 0f, -90f, -180f, 90f };

    private int trialStage;

	void Start()
    {
        if (LoggerCSV.GetInstance().gameMode == LoggerCSV.NORMAL_MODE)
			retrain.gameObject.SetActive(false);
        trialStage = 0;
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
        if (trialStage == 2)
        {
            finishedMessage.SetActive(true);
        }
        else
            trialStage++;
    }

    public void NextScene(){
        SceneManager.LoadScene("main");
    }
}
