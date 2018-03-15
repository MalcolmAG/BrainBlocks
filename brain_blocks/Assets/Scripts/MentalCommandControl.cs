using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MentalCommandControl : MonoBehaviour {

    public Button btnNeutral, btnLeft, btnRight,
                  btnNeutralClear, btnLeftClear,
                  btnRightClear, btnNext;
    public Slider slider;
    public TextMeshProUGUI trainPercentage;
    public TextMeshProUGUI curAction;
    public TextMeshProUGUI status;
    uint userId;
    bool training, rightTrained, leftTrained = false;
    string trainType;
	EmoEngine engine;

    //------------------------------Emotiv Event Functions------------------------------//


    void bindEvents(){
        engine.UserAdded += OnUserAdded;
        engine.MentalCommandTrainingStarted += OnTrainingStarted;
        engine.MentalCommandTrainingSucceeded += OnTrainingSuccessed;
        engine.MentalCommandTrainingCompleted += OnTrainingCompleted;
        engine.MentalCommandEmoStateUpdated += OnMentalCommandEmoStateUpdated;
    }

    //Move TrainingCube and update Current Action UI according to new mental action
    void OnMentalCommandEmoStateUpdated(object sender, EmoStateUpdatedEventArgs args){
        Debug.Log("Change State");
        if (training)
        {
            return;
        }
        EdkDll.IEE_MentalCommandAction_t action = args.emoState.MentalCommandGetCurrentAction();
        //Debug.Log("current action: " + action.ToString());
        switch (action)
        {
            case EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL:
                TrainingCube.action = 0;
                curAction.text = "Current Action: Neutral";
                break;
            case EdkDll.IEE_MentalCommandAction_t.MC_RIGHT:
                TrainingCube.action = 1;
                curAction.text = "Current Action: Right";
                break;
            case EdkDll.IEE_MentalCommandAction_t.MC_LEFT:
                TrainingCube.action = 2;
                curAction.text = "Current Action: Left";
				break;

        }
    }
    private void OnUserAdded(object sender, EmoEngineEventArgs args){
        userId = args.userId;
    }

    void OnTrainingStarted(object sender, EmoEngineEventArgs args)
    {
        status.text = "Training " + trainType;
        training = true;
        ActivateButtons(false);
    }

    void OnTrainingSuccessed(object sender, EmoEngineEventArgs args){
        status.text = "Success! Training " + trainType + " Concluded";
        //should confirm user that they accept that training, in this case auto accept
        engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_ACCEPT);
    }

    void OnTrainingCompleted(object sender, EmoEngineEventArgs args){
        TrainingCube.action = TrainingCube.ACTION_RESET;

        training = false;
        ActivateButtons(true);
        UpdateButtons();
	}

//------------------------------UI Helper Functions------------------------------//
	//Coroutine to control Training_Slider
    //Takes 8 seconds to get to 100%
    IEnumerator UpdateSlider()
	{
		while (true)
		{
			slider.value += Time.deltaTime;
			int percent = (int)(100 * (slider.value / 8));
			trainPercentage.text = percent.ToString() + "%";
			if (percent == 100) break;
			else yield return null;
		}
		yield return new WaitForSeconds(1f);
		slider.value = 0;
		trainPercentage.text = "0%";
        //for testing without emotiv
        TrainingCube.action = TrainingCube.ACTION_RESET;
		ActivateButtons(true);
        UpdateButtons();


	}

    //Shows/Hides Buttons depending on user interaction
    void UpdateButtons () {
        if (leftTrained && rightTrained)
            btnNext.gameObject.SetActive(true);
        else
            btnNext.gameObject.SetActive(false);
        switch(trainType){
            case "Neutral":
                btnNeutralClear.gameObject.SetActive(true);
                btnLeft.gameObject.SetActive(true);
                btnRight.gameObject.SetActive(true);
				break;
            case "clear neutral":
                btnNeutralClear.gameObject.SetActive(false);
                btnRight.gameObject.SetActive(false);
                btnRightClear.gameObject.SetActive(false);
                btnLeft.gameObject.SetActive(false);
                btnLeftClear.gameObject.SetActive(false);
                break;
            case "Right":
                btnRightClear.gameObject.SetActive(true);
                break;
            case "clear right":
                btnRightClear.gameObject.SetActive(false);
                break;
            case "Left":
                btnLeftClear.gameObject.SetActive(true);
                break;
            case "clear left":
                btnLeftClear.gameObject.SetActive(false);
                break;

		}
	}

    //Decativates/Activates buttons depending on training
    void ActivateButtons(bool yes){
        btnNeutral.interactable = yes;
        btnLeft.interactable = yes;
        btnRight.interactable = yes;
        btnNeutralClear.interactable = yes;
        btnLeftClear.interactable = yes;
        btnRightClear.interactable = yes;
    }

//------------------------------UI OnClick Functions------------------------------//

    //Called by Start_Training_Button
    public void CustomStart(){
		LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_TRAIN_START);
		slider.value = 0;
		engine = EmoEngine.Instance;
		bindEvents();
    }

    //Called by Left_Button, Right_Button, and Neutral_Button
	public void TrainAction(string type)
	{
		ActivateButtons(false);

		trainType = type;
        EdkDll.IEE_MentalCommandAction_t toTrain = EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL;
        switch (type)
        {
            case "Left":
                TrainingCube.action = TrainingCube.ACTION_LEFT;
                toTrain = EdkDll.IEE_MentalCommandAction_t.MC_LEFT;
                leftTrained = true;
                break;
            case "Right":
                TrainingCube.action = TrainingCube.ACTION_RIGHT;
                toTrain = EdkDll.IEE_MentalCommandAction_t.MC_RIGHT;
                rightTrained = true;
                break;
        }


        if (type != "Neutral"){
            uint action1 = (uint)EdkDll.IEE_MentalCommandAction_t.MC_LEFT;
            uint action2 = (uint)EdkDll.IEE_MentalCommandAction_t.MC_RIGHT;
            uint listAction = action1 | action2;
            engine.MentalCommandSetActiveActions(userId, listAction);
        }

        StartCoroutine(UpdateSlider());

        engine.MentalCommandSetTrainingAction(userId, toTrain);
		engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_START);
	}

	//Called by Clear_Neutral_Button, Clear_Right_Button & Clear_Left_Button
	public void ClearTraining(string type){

        EdkDll.IEE_MentalCommandAction_t action = EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL;
        trainType = "clear neutral";
        switch(type){
            case "Left":
                action = EdkDll.IEE_MentalCommandAction_t.MC_LEFT;
                trainType = "clear left";
                leftTrained = false;
				break;
            case "Right":
                action = EdkDll.IEE_MentalCommandAction_t.MC_RIGHT;
				trainType = "clear right";
                rightTrained = false;
				break;
        }

        EdkDll.IEE_MentalCommandSetTrainingAction((uint)EmoUserManagement.currentUser, action);
		EdkDll.IEE_MentalCommandSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.IEE_MentalCommandTrainingControl_t.MC_ERASE);
        UpdateButtons();
        status.text= "Cleared " + type + " Training Data";
    }

    //Called by Next_Scene_Button
    public void NextScene(){
        LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_TRAIN_END);
        SceneManager.LoadScene(2);
    }

}
