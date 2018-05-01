using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the acitvation, interactibility of all UI obects in BCI training scene
/// </summary>

public class TrainingUI : MonoBehaviour {

    TrainingController controller;

	//UI
	public Button btnNeutral, btnLeft, btnRight,
				  btnNeutralClear, btnLeftClear,
				  btnRightClear, btnNext, btnResetCube;
	public Slider slider;
    public TextMeshProUGUI trainPercentage, curAction, status, leftCount, rightCount;
    public GameObject clearPanel, timeOutPanel;

    //State Control
    public bool neutralDone, rightDone, leftDone, started, paused = false;
    private int leftTrainCount, rightTrainCount = 0;

	/// <summary>
	/// Initilizes initial values for UI objects
	/// </summary>
	public void InitUI(){
        controller = GameObject.Find("TrainController").GetComponent<TrainingController>();
        slider.value = 0;
        started = true;
        paused = false;
    }

	/// <summary>
	/// Updates notification text under training slidebar
	/// </summary>
    /// <param name="text">Text to be displayed</param>
	public void UpdateStatusText(string text){
        status.text = text;
    }
	/// <summary>
	/// Updates current action text above training cube
	/// </summary>
	/// <param name="text">Current action</param>
	public void UpdateCurrentActionText(string text){
        curAction.text = text;
    }
	/// <summary>
	/// Updates train count texts below clear buttons
	/// </summary>
	void UpdateTrainCounts(){
        leftCount.text = "Train Count: " + leftTrainCount;
        rightCount.text = "Train Count; " + rightTrainCount;
    }

	/// <summary>
	/// Coroutine to control Training_Slider
    /// Takes 8 seconds to get to 100%
	/// </summary>
	public IEnumerator UpdateSlider()
	{
		//XX Start for testing without emotiv
		object s = null;
		EmoEngineEventArgs a = null;
		controller.OnTrainingStarted(s, a);
		//XX END

		while (true)
		{
			slider.value += Time.deltaTime;
			int percent = (int)(100 * (slider.value / slider.maxValue));
			trainPercentage.text = percent.ToString() + "%";
			if (percent == 100) break;
			else yield return null;
		}
		yield return new WaitForSeconds(1f);
		slider.value = 0;
		trainPercentage.text = "0%";

		//XX Start for testing without emotiv
        controller.OnTrainingSuccess(s, a);
		//XX END


	}

	/// <summary>
	/// Updates UI booleans according to user progress
	/// </summary>
    /// <param name="state">state according to button click</param>
    public void UpdateState(string state){
		switch (state)
		{
			case "Neutral":
				neutralDone = true;
				break;
			case "clear neutral":
				neutralDone = false;
				leftDone = false;
				rightDone = false;
				leftTrainCount = 0;
				rightTrainCount = 0;
				ActivateButtons(true);
				break;
            case "Right":
                rightTrainCount++;
                rightDone = true;
                break;
            case "Left":
                leftTrainCount++;
                leftDone = true;
                break;
			case "clear right":
				rightDone = false;
                rightTrainCount = 0;
				break;
			case "clear left":
				leftDone = false;
                leftTrainCount = 0;
				break;
            default:
                break;
		}
        UpdateTrainCounts();
    }
	/// <summary>
	/// Updates all buttons according to user progress
	/// </summary>
    /// <param name="state">state according to button click</param>
	public void UpdateUI(string state)
	{
        UpdateState(state);
		if (leftDone && rightDone)
			btnNext.gameObject.SetActive(true);
		else
			btnNext.gameObject.SetActive(false);
		switch (state)
		{
			case "Neutral":
                //Acitvations
				btnNeutralClear.gameObject.SetActive(true);
				btnLeft.gameObject.SetActive(true);
				btnRight.gameObject.SetActive(true);
				//Interactibility 
				btnNeutral.interactable = false;
				break;
			case "clear neutral":
                //Activations
				btnNeutralClear.gameObject.SetActive(false);

				btnRight.gameObject.SetActive(false);
				btnRightClear.gameObject.SetActive(false);

				btnLeft.gameObject.SetActive(false);
				btnLeftClear.gameObject.SetActive(false);

				//Interactibility
				btnRight.interactable = true;
				btnRightClear.interactable = true;

				btnLeft.interactable = true;
				btnLeftClear.interactable = true;

				ActivateButtons(true);
				break;
			case "Right":
                //Activations
                Debug.Log("right");
				btnRightClear.gameObject.SetActive(true);
				break;
			case "clear right":
                //Activations
				btnRightClear.gameObject.SetActive(false);
                //Interactibility
				btnRight.interactable = true;
				btnRightClear.interactable = true;
				break;
			case "Left":
                Debug.Log("left");
                //Activations
				btnLeftClear.gameObject.SetActive(true);
				break;
			case "clear left":
                //Activations
				btnLeftClear.gameObject.SetActive(false);
				//Interactibility
				btnLeft.interactable = true;
				btnLeftClear.interactable = true;
				break;
		}
	}

	/// <summary>
	/// Decativates/Activates buttons depending on training
	/// </summary>
    /// <param name="yes">Activate Buttons?</param>
	public void ActivateButtons(bool yes)
	{
		if (!neutralDone)
			btnNeutral.interactable = yes;
		btnLeft.interactable = yes;
		btnLeftClear.interactable = yes;
		btnRight.interactable = yes;
		btnRightClear.interactable = yes;
		btnNeutralClear.interactable = yes;
        btnResetCube.interactable = yes;
	}
}
