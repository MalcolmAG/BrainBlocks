using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Controls the acitvation, interactibility of all UI obects in bic_training

public class TrainingUI : MonoBehaviour {

    MentalCommandControl controller;

	//UI
	public Button btnNeutral, btnLeft, btnRight,
				  btnNeutralClear, btnLeftClear,
				  btnRightClear, btnNext, btnRightTrial, btnLeftTrial;
	public Slider slider;
	public TextMeshProUGUI trainPercentage, curAction, status;
	public GameObject leftPrompt, rightPrompt, leftCheckmark, rightCheckmark,
					   trialInfoPanel, acceptTrainPanel, clearPanel;

    //State Control
    public bool neutralDone, leftTrial, rightTrial, rightDone, leftDone = false;


    public void InitUI(){
        controller = GameObject.Find("TrainController").GetComponent<MentalCommandControl>();
        slider.value = 0;
    }

    public void UpdateStatusText(string text){
        status.text = text;
    }
    public void UpdateCurrentActionText(string text){
        curAction.text = text;
    }

	//Coroutine to control Training_Slider
	//Takes 8 seconds to get to 100%
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

    public void UpdateState(string state){
		switch (state)
		{
			case "Neutral":
				neutralDone = true;
				break;
			case "clear neutral":
				neutralDone = false;
				leftTrial = false;
				rightTrial = false;
				leftDone = false;
				rightDone = false;
				ActivateButtons(true);
				break;
			case "clear right":
				rightTrial = false;
				rightDone = false;
				btnRight.interactable = true;
				break;
			case "clear left":
				leftTrial = false;
				leftDone = false;
				break;
			case "left trial start":
				leftTrial = true;
				break;
			case "right trial start":
				rightTrial = true;
				break;
			case "left trial stop":
				leftTrial = false;
				break;
			case "right trial stop":
				rightTrial = false;
				break;
			case "done left":
                leftTrial = false;
                leftDone = true;
				break;
			case "done right":
                rightTrial = false;
                rightDone = true;
				break;
            default:
                break;
		}
    }

	//Shows/Hides UI objects depending on state
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
				if (controller.leftFirst)
					btnLeft.gameObject.SetActive(true);
				else
					btnRight.gameObject.SetActive(true);
				//Interactibility 
				btnNeutral.interactable = false;
				break;
			case "clear neutral":
                //Activations
				btnNeutralClear.gameObject.SetActive(false);

				btnRight.gameObject.SetActive(false);
				btnRightClear.gameObject.SetActive(false);
				rightPrompt.SetActive(false);
				btnRightTrial.gameObject.SetActive(false);
                rightCheckmark.gameObject.SetActive(false);

				btnLeft.gameObject.SetActive(false);
				btnLeftClear.gameObject.SetActive(false);
				leftPrompt.SetActive(false);
				btnLeftTrial.gameObject.SetActive(false);
				leftCheckmark.gameObject.SetActive(false);

				//Interactibility
				btnRight.interactable = true;
				btnRightClear.interactable = true;
				btnRightTrial.interactable = true;

				btnLeft.interactable = true;
				btnLeftClear.interactable = true;
				btnLeftTrial.interactable = true;

                //Renaming
				btnLeftTrial.GetComponentInChildren<TextMeshProUGUI>().text = "Start Trial";
				btnRightTrial.GetComponentInChildren<TextMeshProUGUI>().text = "Start Trial";
				ActivateButtons(true);
				break;
			case "Right":
                //Activations
				btnRightClear.gameObject.SetActive(true);
				btnRightTrial.gameObject.SetActive(true);
				rightCheckmark.SetActive(false);
				break;
			case "clear right":
                //Activations
				btnRightClear.gameObject.SetActive(false);
				rightPrompt.SetActive(false);
				rightCheckmark.gameObject.SetActive(false);
				btnRightTrial.gameObject.SetActive(false);
                //Interactibility
				btnRight.interactable = true;
				btnRightClear.interactable = true;
				btnRightTrial.interactable = true;
				//Renaming
				btnRightTrial.GetComponentInChildren<TextMeshProUGUI>().text = "Start Trial";
				break;
			case "Left":
                //Activations
				btnLeftClear.gameObject.SetActive(true);
				leftCheckmark.SetActive(false);
				btnLeftTrial.gameObject.SetActive(true);
				break;
			case "clear left":
                //Activations
				btnLeftClear.gameObject.SetActive(false);
				leftPrompt.SetActive(false);
				btnLeftTrial.gameObject.SetActive(false);
				leftCheckmark.SetActive(false);
				//Interactibility
				btnLeft.interactable = true;
				btnLeftClear.interactable = true;
				btnLeftTrial.interactable = true;
                //Renaming
                btnLeftTrial.GetComponentInChildren<TextMeshProUGUI>().text = "Start Trial";
				break;
			case "left trial start":
                //Activation
				leftPrompt.SetActive(true);
                //Interactibility
				btnLeft.interactable = false;
				btnLeftClear.interactable = false;
				//Renaming
				btnLeftTrial.GetComponentInChildren<TextMeshProUGUI>().text = "Stop Trial";
				UpdateStatusText("Move the yellow block to match the grey block");
				break;
			case "right trial start":
                //Activation
				rightPrompt.SetActive(true);
                //Interactibility
				btnRight.interactable = false;
				btnRightClear.interactable = false;
				//Renaming
				btnRightTrial.GetComponentInChildren<TextMeshProUGUI>().text = "Stop Trial";
                UpdateStatusText("Move the yellow block to match the grey block");
				break;
			case "left trial stop":
                //Activation
				leftPrompt.SetActive(false);
                //Interactibility
				btnLeft.interactable = true;
				btnLeftClear.interactable = true;
                //Renaming
                btnLeftTrial.GetComponentInChildren<TextMeshProUGUI>().text = "Start Trial";
				UpdateStatusText("Trial Terminated");
				break;
			case "right trial stop":
                //Activation
                rightPrompt.SetActive(false);
				//Interactibility
				btnRight.interactable = true;
				btnRightClear.interactable = true;
				//Renaming
				btnRightTrial.GetComponentInChildren<TextMeshProUGUI>().text = "Start Trial";
				UpdateStatusText("Trial Terminated");
				break;
			case "done left":
				//Activation
				leftCheckmark.SetActive(true);
				leftPrompt.SetActive(false);
				if (controller.leftFirst)
					btnRight.gameObject.SetActive(true);
                //Interactibility
				btnLeftTrial.interactable = false;
				//Renaming
				UpdateStatusText("Left Command Adequately Trained");
				btnLeftTrial.GetComponentInChildren<TextMeshProUGUI>().text = "Trial Done";
				break;
			case "done right":
				//Activation
				rightCheckmark.SetActive(true);
				rightPrompt.SetActive(false);
				if (!controller.leftFirst)
					btnLeft.gameObject.SetActive(true);
				//Interactibility
				btnRightTrial.interactable = false;
                //Renaming
				UpdateStatusText("Right Command Adequately Trained");
				btnRightTrial.GetComponentInChildren<TextMeshProUGUI>().text = "Trial Done";
				break;
		}
	}

	//Decativates/Activates buttons depending on training
	public void ActivateButtons(bool yes)
	{
		//Debug.Log("ActivateButtons(" + yes + ")");
		if (!neutralDone)
			btnNeutral.interactable = yes;
		if (!leftDone)
		{
			btnLeft.interactable = yes;
			btnLeftClear.interactable = yes;
            btnLeftTrial.interactable = yes;
		}
		if (!rightDone)
		{
            btnRightTrial.interactable = yes;
			btnRight.interactable = yes;
			btnRightClear.interactable = yes;
		}
		btnNeutralClear.interactable = yes;
	}
}
