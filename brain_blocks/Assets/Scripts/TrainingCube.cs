using UnityEngine;
using System.Collections;

public class TrainingCube : MonoBehaviour {
    
    public float speed = .2f;
    public readonly int ACTION_RESET = -1;
    public readonly int ACTION_NEUTRAL = 0;
    public readonly int ACTION_RIGHT = 1;
    public readonly int ACTION_LEFT = 2;
    private TrainingUI UI;
    int action = 0;
    private float startPos, offset;
    // Use this for initialization
    private void Start()
    {
        UI = GameObject.Find("TrainController").GetComponent<TrainingUI>();
        startPos = transform.position.x;
        offset = 11;
    }

    public void SetAciton(int a){
        if( a == ACTION_RESET){
            transform.position = new Vector3(4, 15, 0);
			action = ACTION_NEUTRAL;
        }
        else
            action = a;
    }
    //Controls animation of BCI training Block
    void Update () {
        //Neutral action = dont move
        //Right
        if (action == ACTION_RIGHT)
        {
            if (transform.localPosition.x < startPos + offset)
            {
                transform.Translate(speed * Time.deltaTime * 10, 0, 0);
            }
            if (UI.rightTrial && transform.position.x > startPos + 5){
                LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_TRAINING_TRIAL_PASS_R);
                UI.UpdateUI("done right");
            }

        }
        //Left
        else if(action == ACTION_LEFT){
			if (transform.localPosition.x > startPos - offset)
			    transform.Translate( -speed * Time.deltaTime * 10, 0, 0);
            if (UI.leftTrial && transform.position.x < startPos - 5){
                LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_TRAINING_TRIAL_PASS_L);
                UI.UpdateUI("done left");
            }
        }
    }
}
