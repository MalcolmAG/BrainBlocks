using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TrainingUIController : MonoBehaviour {

    Dictionary<int, string> d = new Dictionary<int, string>();

	EmoEngine engine;
    public Button btnNeutral;
    public TextMeshProUGUI status;
	uint userId;
	bool training = false;
	float trainingInterval = 0.0625f; //duration 8s
	void Start()
	{
		engine = EmoEngine.Instance;
		BindEvents();
		d.Add(0, "Neutral");
		d.Add(5, "Left");
		d.Add(6, "Right");
	}

	void BindEvents()
	{
		engine.UserAdded += onUserAdded;
		engine.UserRemoved += onUserRemoved;
		engine.MentalCommandTrainingStarted += onTrainingStarted;
		//engine.MentalCommandTrainingSucceeded += onTrainingSuccessed;
		engine.MentalCommandTrainingCompleted += onTrainingCompleted;
		engine.MentalCommandEmoStateUpdated += onMentalCommandEmoStateUpdated;
	}


	void onMentalCommandEmoStateUpdated(object sender, EmoStateUpdatedEventArgs args)
	{
		//if (training)
		//{
		//	return;
		//}
		//EdkDll.IEE_MentalCommandAction_t action = args.emoState.MentalCommandGetCurrentAction();
		//Debug.Log("current action: " + action.ToString());
		//switch (action)
		//{
		//	case EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL:
		//		status.text = "CurrentAction: Neutral";
		//		break;
  //          case EdkDll.IEE_MentalCommandAction_t.MC_LEFT:
		//		status.text = "CurrentAction: Left";
		//		break;
		//}
	}

	private void onUserAdded(object sender, EmoEngineEventArgs args)
	{
		userId = args.userId;
		Debug.Log("Current User: " + userId.ToString());
	}
	private void onUserRemoved(object sender, EmoEngineEventArgs args)
	{

	}

	void onTrainingStarted(object sender, EmoEngineEventArgs args)
	{
		status.text = "Training started";
		btnNeutral.gameObject.SetActive(false);
		training = true;
	}

	//void onTrainingSuccessed(object sender, EmoEngineEventArgs args)
	//{
	//	status.text = "Training successed";
	//	//should confirm user that they accept that training, in this case auto accept
	//	engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_ACCEPT);
	//}

	void onTrainingCompleted(object sender, EmoEngineEventArgs args)
	{
		status.text = "Training completed";
		btnNeutral.gameObject.SetActive(true);
		training = false;
	}
	public void NextScene(){
        SceneManager.LoadScene(2);
    }

    public void TrainAction(int id){
		EmoMentalCommand.EnableMentalCommandAction(EmoMentalCommand.MentalCommandActionList[id], true);
		EmoMentalCommand.EnableMentalCommandActionsList();
		EmoMentalCommand.StartTrainingMentalCommand(EmoMentalCommand.MentalCommandActionList[id]);
        Debug.Log("Training " + d[id]);
    }

    public void ClearAction(int id){
        EdkDll.IEE_MentalCommandSetTrainingAction((uint)EmoUserManagement.currentUser, EmoMentalCommand.MentalCommandActionList[id]);
		EdkDll.IEE_MentalCommandSetTrainingControl((uint)EmoUserManagement.currentUser, EdkDll.IEE_MentalCommandTrainingControl_t.MC_ERASE);
		Debug.Log("Clearing " + d[id]);
    }

    private void Update()
    {
        Debug.Log("Neutral: " +EmoMentalCommand.MentalCommandActionPower[0]);
		Debug.Log("Left: " + EmoMentalCommand.MentalCommandActionPower[5]);

	}
}
