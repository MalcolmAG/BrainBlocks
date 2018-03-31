using UnityEngine;
using System.Collections;

public class TrainingCube : MonoBehaviour {
    
    public float speed = .2f;
    public readonly int ACTION_RESET = -1;
    public readonly int ACTION_NEUTRAL = 0;
    public readonly int ACTION_RIGHT = 1;
    public readonly int ACTION_LEFT = 2;
    public bool leftDone, rightDone, righttrial, lefttrial = false;
    private MentalCommandControl control;
    int action = 0;
    private float startPos, offset;
    // Use this for initialization
    private void Start()
    {
        control = GameObject.Find("TrainController").GetComponent<MentalCommandControl>();
        startPos = transform.position.x;
        offset = 11;
    }

    public void SetAciton(int a){
        action = a;
    }
    //Controls animation of BCI training Block
    void Update () {
        if(action == ACTION_RESET){
            transform.position = new Vector3(4, 15, 0);
            action = ACTION_NEUTRAL;
        }
        //Neutral
        if (action == ACTION_NEUTRAL)
        {

            if (transform.localPosition.x > 4.3)
                transform.Translate(-speed * Time.deltaTime * 10, 0, 0);
            else if (transform.localPosition.x < 3.8)
                transform.Translate(speed * Time.deltaTime * 10, 0, 0);
        }
        //Right
        else if (action == ACTION_RIGHT)
        {
            if (transform.localPosition.x < startPos + offset)
            {
                transform.Translate(speed * Time.deltaTime * 10, 0, 0);
            }
            if (righttrial && transform.position.x > startPos + 5){
                LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_TRAINING_TRIAL_PASS_R);
                rightDone = true;
                righttrial = false;
                control.UpdateUI("done right");
            }

        }
        //Left
        else if(action == ACTION_LEFT){
			if (transform.localPosition.x > startPos - offset)
			    transform.Translate( -speed * Time.deltaTime * 10, 0, 0);
            if (lefttrial && transform.position.x < startPos - 5){
				LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_TRAINING_TRIAL_PASS_R);
				leftDone = true;
                lefttrial = false;
                control.UpdateUI("done left");
            }
        }
    }
}
