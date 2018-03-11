using UnityEngine;
using System.Collections;

public class cube : MonoBehaviour {

    public static int action = 0;
    public static float speed = 7.0f / 8.0f;
    public static readonly int ACTION_NEUTRAL = 0;
    public static readonly int ACTION_RIGHT = 1;
    public static readonly int ACTION_LEFT = 2;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {   
        //Neutral
        if (action == ACTION_NEUTRAL)
        {
            if (transform.localPosition.x > 4 )
				transform.Translate(0, 0, -speed * Time.deltaTime * 10);
            else if(transform.localPosition.x < 4)
				transform.Translate(0, 0, speed * Time.deltaTime * 10);
		}
        //Right
        else if(action == ACTION_RIGHT){
			if (transform.localPosition.x < 7)
			{
				transform.Translate(speed * Time.deltaTime * 10, 0, 0);
			}
			else
			{
				transform.localPosition = new Vector3(4, 0, 0);
			}
        }
        //Left
        else if(action == ACTION_LEFT){
			if (transform.localPosition.x > -7)
			{
				transform.Translate( -speed * Time.deltaTime * 10, 0, 0);
			}
			else
			{ 
				transform.localPosition = new Vector3(4, 0, 0);
			}
        }
	}
}
