using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

    public GameObject[] options;
    public GameObject preview;
    private GameObject next_group;

    void Start () {
        //Creates first "preview" group
		int next = Random.Range(0, options.Length);
		next_group = Instantiate(options[next], preview.transform.position, Quaternion.identity);
        CreateNext();
	}
	
    //Spawns "preview" group at top of game area
    //Randmomly chooses next "preview" group
    public void CreateNext(){
        next_group.transform.position = transform.position;
        next_group.AddComponent<Set>();
		int next = Random.Range(0, options.Length);
		next_group = Instantiate(options[next], preview.transform.position, Quaternion.identity);
    }
}
