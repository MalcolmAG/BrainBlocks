using UnityEngine;
using System.Collections;

public class cube : MonoBehaviour {

    public static int action = 0;
    public static float speed = .2f;
    public static readonly int ACTION_RESET = -1;
    public static readonly int ACTION_NEUTRAL = 0;
    public static readonly int ACTION_RIGHT = 1;
    public static readonly int ACTION_LEFT = 2;
	// Use this for initialization
    public static void ResetPos(){
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
        else if(action == ACTION_RIGHT){
			if (transform.localPosition.x < 15)
			{
				transform.Translate(speed * Time.deltaTime * 10, 0, 0);
			}
        }
        //Left
        else if(action == ACTION_LEFT){
			if (transform.localPosition.x > -7)
			{
				transform.Translate( -speed * Time.deltaTime * 10, 0, 0);
			}
        }
    }
}
