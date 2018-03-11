using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class MentalCommandControl : MonoBehaviour {

    public Button btnNeutral, btnLeft, btnRight;
    public Slider slider;
    public TextMeshProUGUI trainPercentage;
    public TextMeshProUGUI curAction;
    public TextMeshProUGUI status;

    uint userId;
    bool training = false;
    string trainType;
	EmoEngine engine;


	void Start () {
        slider.value = 0;
        engine = EmoEngine.Instance;
        bindEvents();
	}

    IEnumerator UpdateSlider(){
        while (true){
            slider.value += Time.deltaTime;
            int percent = (int)(100 * (slider.value / 8));
            trainPercentage.text = percent.ToString() + "%";
            if (percent == 100) break;
            else yield return null;
        }
        yield return new WaitForSeconds(1f);
        slider.value = 0;
        trainPercentage.text = "0%";
        btnLeft.gameObject.SetActive(true);
        btnRight.gameObject.SetActive(true);
    }

    void bindEvents(){
        engine.UserAdded += onUserAdded;
        engine.UserRemoved += onUserRemoved;
        engine.MentalCommandTrainingStarted += onTrainingStarted;
        engine.MentalCommandTrainingSucceeded += onTrainingSuccessed;
        engine.MentalCommandTrainingCompleted += onTrainingCompleted;
        engine.MentalCommandEmoStateUpdated += onMentalCommandEmoStateUpdated;
    }


    void onMentalCommandEmoStateUpdated(object sender, EmoStateUpdatedEventArgs args){
        if (training)
        {
            return;
        }
        EdkDll.IEE_MentalCommandAction_t action = args.emoState.MentalCommandGetCurrentAction();
        Debug.Log("current action: " + action.ToString());
        switch (action)
        {
            case EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL:
                cube.action = 0;
                curAction.text = "CurrentAction: Neutral";
                break;
            case EdkDll.IEE_MentalCommandAction_t.MC_RIGHT:
                cube.action = 1;
                curAction.text = "CurrentAction: Pushing";
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
        btnNeutral.gameObject.SetActive(false);
        btnLeft.gameObject.SetActive(false);
        btnLeft.gameObject.SetActive(false);
        training = true;
    }

    void onTrainingSuccessed(object sender, EmoEngineEventArgs args){
        status.text = "Success! Training " + trainType +" Concluded";
        //should confirm user that they accept that training, in this case auto accept
        engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_ACCEPT);
    }

    void onTrainingCompleted(object sender, EmoEngineEventArgs args){
        training = false;
    }
	// Update is called once per frame
	void Update () {
	
	}

    public void TrainNeutral(){
        training = true;
        trainType = "Neutral";
        cube.action = cube.ACTION_NEUTRAL;

        StartCoroutine(UpdateSlider());

		//no need to call MentalCommandSetActiveActions with neutral action
		engine.MentalCommandSetTrainingAction(userId, EdkDll.IEE_MentalCommandAction_t.MC_NEUTRAL);
		engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_START);
	}
	public void TrainLeft()
	{
		training = true;
        trainType = "Left";
        cube.action = cube.ACTION_LEFT;

        StartCoroutine(UpdateSlider());

        engine.MentalCommandSetActiveActions(userId, (uint)EdkDll.IEE_MentalCommandAction_t.MC_LEFT);
		engine.MentalCommandSetTrainingAction(userId, EdkDll.IEE_MentalCommandAction_t.MC_LEFT);
		engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_START);
	}
    public void TrainRight(){
        training = true;
        trainType = "Right";
        cube.action = cube.ACTION_RIGHT;

		StartCoroutine(UpdateSlider());

		engine.MentalCommandSetActiveActions(userId, (uint)EdkDll.IEE_MentalCommandAction_t.MC_RIGHT);
        engine.MentalCommandSetTrainingAction(userId, EdkDll.IEE_MentalCommandAction_t.MC_RIGHT);
        engine.MentalCommandSetTrainingControl(userId, EdkDll.IEE_MentalCommandTrainingControl_t.MC_START);
    }

}
