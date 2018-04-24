using UnityEngine;
using System.Collections;

/// <summary>
/// Controls movement and trial completion of training cube
/// </summary>
public class TrainingCube : MonoBehaviour {
    
    public float speed = .2f;
    public readonly int ACTION_RESET = -1;
    public readonly int ACTION_NEUTRAL = 0;
    public readonly int ACTION_RIGHT = 1;
    public readonly int ACTION_LEFT = 2;
    int action = 0;
    private float startPos, offset;


	/// <summary>
	/// Initilizes Training cube variables
	/// </summary>
	private void Start()
    {
        startPos = transform.position.x;
        offset = 11;
    }

	/// <summary>
	/// Assings current action of TrainingCube
	/// </summary>
    /// <param name="a">Action to be set</param>
	public void SetAciton(int a){
        if( a == ACTION_RESET){
            transform.position = new Vector3(4, 15, 0);
			action = ACTION_NEUTRAL;
        }
        else
            action = a;
    }
	/// <summary>
	/// Controls animation of TrainingCube
	/// </summary>
	void Update () {
        //Neutral action = dont move
        //Right
        if (action == ACTION_RIGHT)
        {
            if (transform.localPosition.x < startPos + offset)
            {
                transform.Translate(speed * Time.deltaTime * 10, 0, 0);
            }

        }
        //Left
        else if(action == ACTION_LEFT){
			if (transform.localPosition.x > startPos - offset)
			    transform.Translate( -speed * Time.deltaTime * 10, 0, 0);
        }
    }
}
