using UnityEngine;
using System.Collections;

public class TrainingCube : MonoBehaviour {

    public int action = 0;
    public float speed = .2f;
    public readonly int ACTION_RESET = -1;
    public readonly int ACTION_NEUTRAL = 0;
    public readonly int ACTION_RIGHT = 1;
    public readonly int ACTION_LEFT = 2;
    public bool leftDone, rightDone, rightTrail, leftTrail = false;
    private MentalCommandControl control;
    private float startPos, offset;
    // Use this for initialization
    private void Start()
    {
        control = GameObject.Find("TrainController").GetComponent<MentalCommandControl>();
        startPos = transform.position.x;
        offset = 11;
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
            if (rightTrail && transform.position.x > startPos + 5){
                rightDone = true;
                rightTrail = false;
                control.UpdateUI("done right");
            }

        }
        //Left
        else if(action == ACTION_LEFT){
			if (transform.localPosition.x > startPos - offset)
			    transform.Translate( -speed * Time.deltaTime * 10, 0, 0);
            if (leftTrail && transform.position.x < startPos - 5){
                leftDone = true;
                leftTrail = false;
                control.UpdateUI("done left");
            }
        }
    }
}
