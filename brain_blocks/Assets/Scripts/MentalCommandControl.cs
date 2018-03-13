using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class MentalCommandControl : MonoBehaviour {

    public Button btnNeutral, btnLeft, btnRight,
                  btnNeutralClear, btnLeftClear,
                  btnRightClear;
    public Slider slider;
    public TextMeshProUGUI trainPercentage;
    public TextMeshProUGUI curAction;
    public TextMeshProUGUI status;
    public bool print;
    uint userId;
    bool training = false;
    string trainType;
	EmoEngine engine;


	void Start () {
        slider.value = 0;
        engine = EmoEngine.Instance;
        bindEvents();
	}

    //------------------------------Emotiv Event Functions------------------------------//


    void bindEvents(){
        engine.UserAdded += onUserAdded;
        engine.UserRemoved += onUserRemoved;
        engine.MentalCommandTrainingStarted += onTrainingStarted;
        engine.MentalCommandTrainingSucceeded += onTrainingSuccessed;
        engine.MentalCommandTrainingCompleted += onTrainingCompleted;
        engine.MentalCommandEmoStateUpdated += onMentalCommandEmoStateUpdated;
    }


    void onMentalCommandEmoStateUpdated(object sender, EmoStateUpdatedEventArgs args){
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
                cube.action = 0;
                curAction.text = "CurrentAction: Neutral";
                break;
            case EdkDll.IEE_MentalCommandAction_t.MC_RIGHT:
                cube.action = 1;
                curAction.text = "CurrentAction: Right";
                break;
            case EdkDll.IEE_MentalCommandAction_t.MC_LEFT:
                cube.action = 2;
                curAction.text = "CurrentAction: Left";
				break;

        }
    }
    private void onUserAdded(object sender, EmoEngineEventArgs args){
        userId = args.userId;
        Debug.Log("Current User: " + userId.ToString());
    }
    private void onUserRemoved(object sender, EmoEngineEventArgs args){
        
    }

    void onTrainingStarted(object sender, EmoEngineEventArgs args)
    {
        status.text = "Training " + trainType;
        training = true;
    }

    void onTrainingSuccessed(object sender, EmoEngineEventArgs args){
        status.text = "Success! Training " + trainType +" Concluded";
        //should confirm user that they accept that training, in this case auto accept
        engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_ACCEPT);
    }

    void onTrainingCompleted(object sender, EmoEngineEventArgs args){
        training = false;
        UpdateButtons();
        cube.action = cube.ACTION_RESET;
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

	}

    //Activates/Deactivates Buttons depending on user interaction
    void UpdateButtons () {
        switch(trainType){
            case "Neutral":
                btnNeutralClear.interactable = true;
                btnLeft.interactable = true;
                btnRight.interactable = true;
				break;
            case "clear neutral":
                btnNeutralClear.interactable = false;
                btnRight.interactable = false;
                btnRightClear.interactable = false;
                btnLeft.interactable = false;
                btnLeftClear.interactable = false;
                break;
            case "Right":
                btnRightClear.interactable = true;
                break;
            case "clear right":
                btnRightClear.interactable = false;
                break;
            case "Left":
                btnLeftClear.interactable = true;
                break;
            case "clear left":
                btnLeftClear.interactable = false;
                break;

		}
	}

//------------------------------UI OnClick Functions------------------------------//


    //Called by Neutral_Button
    public void TrainNeutral(){
        training = true;
        trainType = "Neutral";
        cube.action = cube.ACTION_NEUTRAL;

        StartCoroutine(UpdateSlider());

		//no need to call MentalCommandSetActiveActions with neutral action
		engine.MentalCommandSetTrainingAction(userId, EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL);
		engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_START);
	}

    //Called by Left_Button
	public void TrainLeft()
	{
        uint action1 = (uint)EdkDll.IEE_MentalCommandAction_t.MC_LEFT;
        uint action2 = (uint)EdkDll.IEE_MentalCommandAction_t.MC_RIGHT;
		uint listAction = action1 | action2;

		training = true;
        trainType = "Left";
        cube.action = cube.ACTION_LEFT;

        StartCoroutine(UpdateSlider());

        engine.MentalCommandSetActiveActions(userId, listAction);
		engine.MentalCommandSetTrainingAction(userId, EdkDll.IEE_MentalCommandAction_t.MC_LEFT);
		engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_START);
	}

    //Called by Right_Button
    public void TrainRight(){
        training = true;
        trainType = "Right";
        cube.action = cube.ACTION_RIGHT;

		StartCoroutine(UpdateSlider());
        uint action1 = (uint)EdkDll.IEE_MentalCommandAction_t.MC_LEFT;
        uint action2 = (uint)EdkDll.IEE_MentalCommandAction_t.MC_RIGHT;
		uint listAction = action1 | action2;
        engine.MentalCommandSetActiveActions(userId, (uint) EdkDll.IEE_MentalCommandAction_t.MC_RIGHT);
		engine.MentalCommandSetTrainingAction(userId, EdkDll.IEE_MentalCommandAction_t.MC_RIGHT);
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
				break;
            case "Right":
                action = EdkDll.IEE_MentalCommandAction_t.MC_RIGHT;
				trainType = "clear right";
				break;
        }

        EdkDll.IEE_MentalCommandSetTrainingAction((uint)EmoUserManagement.currentUser, action);
		EdkDll.IEE_MentalCommandSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.IEE_MentalCommandTrainingControl_t.MC_ERASE);
        UpdateButtons();
        status.text= "Cleared " + type + " Training Data";
    }

}
