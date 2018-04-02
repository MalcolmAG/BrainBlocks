using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MentalCommandControl : MonoBehaviour {
    //Control
    public TrainingCube cube;
    uint userId;
    public bool firstTime = true;
    public bool training, leftFirst, inputRecieved, acceptTraining = false;
    public string trainType;
	EmoEngine engine;
    TrainingUI UI;


    //XX Start For testing without EMOTIV
    //private void Update()
    //{
    //    if (!training)
    //    {
    //        if (Input.GetKey(KeyCode.LeftArrow))
    //            cube.SetAciton(cube.ACTION_LEFT);
    //        else if (Input.GetKey((KeyCode.RightArrow)))
    //            cube.SetAciton(cube.ACTION_RIGHT);
    //        else
    //            cube.SetAciton(cube.ACTION_NEUTRAL);
    //    }
    //}
    // XX End

//------------------------------Emotiv Event Functions------------------------------//

    //Binds following local functions(right side) to EmoEngine functions(left side)
    void BindEvents(){
        engine.UserAdded += OnUserAdded;
        engine.MentalCommandTrainingStarted += OnTrainingStarted;
        engine.MentalCommandTrainingSucceeded += OnTrainingSuccess;
        engine.MentalCommandTrainingFailed += OnTrainingFailed;
		engine.MentalCommandTrainingCompleted += OnTrainingAccepted;
        engine.MentalCommandTrainingRejected += OnTrainingRejected;
        engine.MentalCommandEmoStateUpdated += OnMentalCommandEmoStateUpdated;
    }
	//Uninds following local functions(right side) to EmoEngine functions(left side)
	void UnbindEvents(){
		engine.UserAdded -= OnUserAdded;
		engine.MentalCommandTrainingStarted -= OnTrainingStarted;
        engine.MentalCommandTrainingSucceeded -= OnTrainingSuccess;
        engine.MentalCommandTrainingFailed -= OnTrainingFailed;
		engine.MentalCommandTrainingCompleted -= OnTrainingAccepted;
		engine.MentalCommandTrainingRejected -= OnTrainingRejected;
		engine.MentalCommandEmoStateUpdated -= OnMentalCommandEmoStateUpdated;
    }

    void OnTrainingFailed(object sender, EmoEngineEventArgs args){
        Debug.Log("In Failed");
		UI.UpdateStatusText(trainType + " Failed Due to Noisy Signal, Please Try Again");
		cube.SetAciton(cube.ACTION_RESET);
		UI.ActivateButtons(true);
		training = false;
    }

    //Move cube and update Current Action UI according to new mental action
    void OnMentalCommandEmoStateUpdated(object sender, EmoStateUpdatedEventArgs args){
        if (training) return; //do not update during training
        EdkDll.IEE_MentalCommandAction_t action = args.emoState.MentalCommandGetCurrentAction();
        //Move Block and Update UI text
        switch (action)
        {
            case EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL:
                cube.SetAciton(cube.ACTION_NEUTRAL);
                UI.UpdateCurrentActionText("Current Action: Neutral");
                break;
            case EdkDll.IEE_MentalCommandAction_t.MC_RIGHT:
                cube.SetAciton(cube.ACTION_RIGHT);
                UI.UpdateCurrentActionText("Current Action: Right");
                break;
            case EdkDll.IEE_MentalCommandAction_t.MC_LEFT:
                cube.SetAciton(cube.ACTION_LEFT);
                UI.UpdateCurrentActionText("Current Action: Left");
				break;

        }
    }

	//Event function called by EmoEngine when EPOCH is connected
	void OnUserAdded(object sender, EmoEngineEventArgs args){
        userId = args.userId;
    }

	//Event function called by EmoEngine before training period starts
	public void OnTrainingStarted(object sender, EmoEngineEventArgs args)
    {
        UI.UpdateStatusText( "Training " + trainType);
        training = true;
        UI.ActivateButtons(false);
    }

    //Event function called by EmoEngine when training period ends
    public void OnTrainingSuccess(object sender, EmoEngineEventArgs args){
		Debug.Log("In Success");
		StartCoroutine(AcceptTraining());
    }

	//Event function called by EmoEngine when training is accepted
	void OnTrainingAccepted(object sender, EmoEngineEventArgs args){
		Debug.Log("In Accepted");
        UI.UpdateStatusText( "Success! Training " + trainType + " Concluded");
		training = false;
		UI.ActivateButtons(true);

        if (!firstTime || trainType == "Neutral")
        {
            ResetCube();
        }
        else
            cube.SetAciton(cube.ACTION_RESET);
		//Shows information about training trials after
		// accepting the first right or left data
		if (firstTime && trainType != "Neutral")
		{
			firstTime = false;
			UI.trialInfoPanel.SetActive(true);

		}
	}

	//Event function called by EmoEngine when training is rejected
	void OnTrainingRejected(object sender, EmoEngineEventArgs args){
		Debug.Log("In Rejected");
        UI.UpdateStatusText( trainType + " Data Rejected");
        cube.SetAciton(cube.ACTION_RESET);
        UI.ActivateButtons(true);
        training = false;
	} 

    //Waits on user to commit or reject most recent training data
	IEnumerator AcceptTraining()
	{
        UI.acceptTrainPanel.SetActive(true);
		while (!inputRecieved) yield return null;
		if (acceptTraining) {
            LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_TRAINING_ACCEPT);
			//XX Start
			//object s = null;
			//EmoEngineEventArgs a = null;
			//OnTrainingAccepted(s,a);
			//XX end
			Debug.Log("Coroutine: ACCEPTING");
			engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_ACCEPT);
		}
		else{
			Debug.Log("Coroutine: REJECTING");
			LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_TRAINING_REJECT);
			//XX Start
			//object s = null;
			//EmoEngineEventArgs a = null;
            //OnTrainingRejected  (s, a);
			//XX end
			engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_REJECT);
		}
        inputRecieved = false;
	}

	//Clear all previous training data
	void ClearAllTraining()
	{

		EdkDll.IEE_MentalCommandAction_t[] actions = {EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL,
													  EdkDll.IEE_MentalCommandAction_t.MC_RIGHT,
													  EdkDll.IEE_MentalCommandAction_t.MC_LEFT};
		for (int i = 0; i < 3; i++)
		{
			EdkDll.IEE_MentalCommandSetTrainingAction((uint)EmoUserManagement.currentUser, actions[i]);
			EdkDll.IEE_MentalCommandSetTrainingControl((uint)EmoUserManagement.currentUser,
													   EdkDll.IEE_MentalCommandTrainingControl_t.MC_ERASE);
		}
	}

    //------------------------------UI OnClick Functions------------------------------//

    //Called by Start_Training_Button
    public void CustomStart()
    {
        LoggerCSV logger = LoggerCSV.GetInstance();
        logger.AddEvent(LoggerCSV.EVENT_TRAINSTAGE_START);

        UI = GetComponent<TrainingUI>();
        cube = GameObject.Find("Block").GetComponent<TrainingCube>();
        leftFirst = logger.counterBalanceID == 1
                          || logger.counterBalanceID == 2;
        engine = EmoEngine.Instance;

        UI.InitUI();
        //Destroy Previous Training Data
        ClearAllTraining();
		BindEvents();

    }

    //Called by Left_Button, Right_Button, and Neutral_Button
	public void TrainAction(string type)
	{
        LoggerCSV logger = LoggerCSV.GetInstance();
		trainType = type;
        EdkDll.IEE_MentalCommandAction_t toTrain = EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL;
        cube.SetAciton(cube.ACTION_RESET);
        switch (type)
        {
            case "Left":
                //cube.SetAciton(cube.ACTION_LEFT);
                toTrain = EdkDll.IEE_MentalCommandAction_t.MC_LEFT;
                logger.AddEvent(LoggerCSV.EVENT_TRAINING_L);
                break;
            case "Right":
                //   cube.SetAciton(cube.ACTION_RIGHT);
				toTrain = EdkDll.IEE_MentalCommandAction_t.MC_RIGHT;
				logger.AddEvent(LoggerCSV.EVENT_TRAINING_R);
				break;
            default:
                logger.AddEvent(LoggerCSV.EVENT_TRAINING_N);
                break;
        }

		StartCoroutine(UI.UpdateSlider());

        if (type != "Neutral")
        {
            EmoMentalCommand.EnableMentalCommandAction(toTrain, true);
            EmoMentalCommand.EnableMentalCommandActionsList();
        }
        EmoMentalCommand.StartTrainingMentalCommand(toTrain);
	}

    //Called by Clear_Neutral_Button, Clear_Right_Button & Clear_Left_Button
    public void ClearTrainingCheck(string type)
    {
        switch (type)
        {
            case "Left":
                trainType = "clear left";
                break;
            case "Right":
                trainType = "clear right";
                break;
            default:
                trainType = "clear neutral";
                break;
        }
        UI.clearPanel.SetActive(true);

    }

	//Called by Accept_Clear_Button
	public void ClearTraining(){
        LoggerCSV logger = LoggerCSV.GetInstance();
        string statusText = "Neutral";
		EdkDll.IEE_MentalCommandAction_t action = EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL;
        switch (trainType)
		{
			case "clear left":
                statusText = "Left";
				action = EdkDll.IEE_MentalCommandAction_t.MC_LEFT;
				trainType = "clear left";
                logger.AddEvent(LoggerCSV.EVENT_TRAINING_CLEAR_L);
                break;
			case "clear right":
                statusText = "Right";
				action = EdkDll.IEE_MentalCommandAction_t.MC_RIGHT;
				trainType = "clear right";
				logger.AddEvent(LoggerCSV.EVENT_TRAINING_CLEAR_R);
				break;
            default:
                logger.AddEvent(LoggerCSV.EVENT_TRAINING_CLEAR_N);
                UI.UpdateStatusText("Current Aciton: None");
                ClearAllTraining();
                UI.UpdateStatusText("Cleared " + statusText + " Training Data");
                UI.UpdateUI(trainType);
                return;
		}
		EdkDll.IEE_MentalCommandSetTrainingAction((uint)EmoUserManagement.currentUser, action);
		EdkDll.IEE_MentalCommandSetTrainingControl((uint)EmoUserManagement.currentUser,
												   EdkDll.IEE_MentalCommandTrainingControl_t.MC_ERASE);
		UI.UpdateUI(trainType);
        UI.ActivateButtons(true);
        UI.UpdateStatusText( "Cleared " + statusText + " Training Data");
    }

    //Called by Accept_Training_Button and Reject Training Button
    public void AcceptTrainClick(bool yes){
        inputRecieved = true;
        acceptTraining = yes;
    }

    //Called by Next_Scene_Button
    public void NextScene(){
        LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_TRAINSTAGE_END);
        UnbindEvents();
        SceneManager.LoadScene(2);
    }

    //Called by Right_Trial_Button and Left_Trial_Button
    public void StartEndTrial(bool left){
        cube.SetAciton(cube.ACTION_RESET);
        if (left){
            //Trial is ongoing
            if (UI.leftTrial)
                UI.UpdateUI("left trial stop");
			//Trial hasn't started
			else
                UI.UpdateUI("left trial start");
        }
        else{
            //Trial is ongoing
            if (UI.rightTrial)
                UI.UpdateUI("right trial stop");
            //Trial hasn't started
            else
                UI.UpdateUI("right trial start");
        }
    }

	//Called by Continue_Training_Button
    //Ensures Cube is reset before starting trials
	public void ResetCube(){
		cube.SetAciton(cube.ACTION_RESET);
		UI.UpdateUI(trainType);
    }
}