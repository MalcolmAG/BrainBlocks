using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour {

    public static int score;

	public GameObject finishedMessage;
    public GameObject pauseMessage;

    private bool checking;
    private bool midWayReached;
	private float  startingTime;
    public float halfAllottedTime;


	// Use this for initialization
	void Start () {
        pauseMessage.SetActive(false);
        finishedMessage.SetActive(false);
        midWayReached = false;
        checking = true;
        startingTime = Time.time; 
        score = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if(checking)
            CheckTime();
		
	}

    void CheckTime(){
        Debug.Log(Time.time - startingTime);
        if(Time.time - startingTime > halfAllottedTime){
            if(!midWayReached){
				midWayReached = true;
                checking = false;
				pauseMessage.SetActive(true);
            }
            else{
                finishedMessage.SetActive(true);
            }
        }
    }

    public void FinishGame(){
        SceneManager.LoadScene("menu");
    }

    public void EndPause(){
        pauseMessage.SetActive(false);
        startingTime = Time.time;
        checking = true;
    }
}
