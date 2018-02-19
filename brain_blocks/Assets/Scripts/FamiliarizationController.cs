using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamiliarizationController : MonoBehaviour {

	public GameObject[] options;
    private GameObject group;
    private GameObject target;

	void Start()
	{
		CreateNext();
	}

	//Spawns "preview" group at top of game area
	//Randmomly chooses next "preview" group
	public void CreateNext()
	{
        int i = Random.Range(0, options.Length);
        Vector2 targetPos = new Vector2(Random.Range(0, 9), 0);

        target = Instantiate(options[i], targetPos, Quaternion.identity);
        group = Instantiate(options[i], transform.position, Quaternion.identity);
        group.AddComponent<FamiliarizationSet>();
	}

    public bool CheckOrientation(){
        int t = (int)(Mathf.Rad2Deg * target.transform.rotation.z);
        int g = (int)(Mathf.Rad2Deg * group.transform.rotation.z);
        //deal with UI element
        return t == g;
    }
    public bool CheckPosition(){
        int t = (int)target.transform.position.x;
        int g = (int)group.transform.position.x;
        if(t==g){
            Destroy(target);
            return true;
        }
        return false;
        //deal with UI element
    }
}
