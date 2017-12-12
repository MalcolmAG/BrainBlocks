using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

    public GameObject[] options;

    void Start () {
        CreateNext();
	}
	
    public void CreateNext(){
        int i = Random.Range(0, options.Length);
        Instantiate(options[i],transform.position, Quaternion.identity);
    }
}
