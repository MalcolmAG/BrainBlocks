using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TrainingUIController : MonoBehaviour {

	EmoEngine engine;
    public Button btnNeutral;
    public TextMeshProUGUI status;
	uint userId;
	bool training = false;
	float trainingInterval = 0.0625f; //duration 8s
	void Start()
    {
        LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_TRAIN_START);
    }

	public void NextScene(){
        LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_TRAIN_END);
		SceneManager.LoadScene(2);
    }
}
