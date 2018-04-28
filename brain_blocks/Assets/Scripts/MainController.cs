using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controller for Main gameplay Scene
/// </summary>
public class MainController : MonoBehaviour {

    public static int score;

    public TextMeshProUGUI scoreText;
    public Button pauseButton, retrainButton;

	public GameObject pauseMessage, gameOverMessage;

    public bool paused = false;

//------------------------------Unity & Main Scene Control Functions------------------------------//

    /// <summary>
    /// Initializes relevant variables
    /// </summary>
	void Start () {
        if(GameObject.Find("Persistent_Master") == null){
            retrainButton.gameObject.SetActive(false);
        }
        paused = false;
        score = 0;
        UI_Game();
	}
	
    /// <summary>
    /// Checks if allotted time has been exceeded, updates score
    /// </summary>
	void Update () {
        UpdateScore();
		
	}


    //------------------------------UI OnClick Functions------------------------------//


	/// <summary>
	/// Finishs the game. Called by Done_Button
	/// </summary>
	public void FinishGame(){
        if (GameObject.Find("Persistent_Master") != null)
		{
			GameObject master = GameObject.Find("Persistent_Master");
            Destroy(GameObject.Find("Contact_Quality"));
            master.GetComponent<EmotivControl>().End();
			Destroy(master.GetComponent<EmotivControl>());
			Destroy(master.GetComponent<EmoFacialExpression>());
		}
        LoadScene(0);
    }

	/// <summary>


	/// <summary>
	/// Starts the pause. Called by Pause_Button
	/// </summary>
	public void StartPause(){
        paused = true;
        UI_Pause("pause");
	}

	/// <summary>
	/// Ends the pause. Called by End_Pause_Button
	/// </summary>
	public void EndPause(){
        paused = false;
        UI_Game();
	}

    /// <summary>
    /// Loads the scene.
    /// </summary>
    /// <param name="idx">Index of scene to be loaded</param>
    public void LoadScene(int idx){
        SceneManager.LoadScene(idx);
    }

	//------------------------------UI Helper Functions------------------------------//

	/// <summary>
	/// Sets UI elements for gameplay view
	/// </summary>
	private void UI_Game()
	{
		pauseMessage.SetActive(false);
		pauseButton.gameObject.SetActive(true);
		scoreText.gameObject.SetActive(true);
	}

	/// <summary>
	/// Sets UI elements for paused-game view
	/// </summary>
    /// <param name="type">Type of pause ("midway", "pause", "finished")</param>
	private void UI_Pause(string type)
	{
		scoreText.gameObject.SetActive(false);
		pauseButton.gameObject.SetActive(false);
		switch (type)
		{
			case "gameOver":
				gameOverMessage.SetActive(true);
				return;
			case "pause":
				pauseMessage.SetActive(true);
				return;
			//case "finished":
				//finishedMessage.SetActive(true);
				//return;
		}
	}

    /// <summary>
    /// Game is over, Called by MainSet.cs
    /// </summary>
    public void GameOver(){
        score = 0;
        UI_Pause("gameOver");
    }

	/// <summary>
    /// Updates the score UI element.
    /// </summary>
	public void UpdateScore()
	{
		scoreText.text = "Score: " + score.ToString();
	}
}
