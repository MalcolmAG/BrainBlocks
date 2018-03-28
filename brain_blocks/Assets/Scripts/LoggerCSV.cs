using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class LoggerCSV : MonoBehaviour
{

	public static LoggerCSV instance = null;

    public List<string[]> rows = new List<string[]>();

    public int gameMode = 0;
    public int participantID = -1;

    public static readonly int NORMAL_MODE = 0;
    public static readonly int BCI_MODE = 1;

    public static readonly string EVENT_START_NORMAL = "Start Normal Mode";
    public static readonly string EVENT_END_NORMAL = "End Normal Mode";

    public static readonly string EVENT_UNABLE = "Unable to Complete Stage";

    public static readonly string EVENT_START_BCI = "Start BCI Mode";
    public static readonly string EVENT_END_BCI = "End BCI Mode";

    public static readonly string EVENT_PAUSE_START = "Start Pause";
	public static readonly string EVENT_PAUSE_END = "End Pause";

    public static readonly string EVENT_TRAINSTAGE_START = "Start BCI Training Stage";
    public static readonly string EVENT_TRAINSTAGE_END = "End BCI Training Stage";

	public static readonly string EVENT_TRAINING_N = "Training Neutral";
	public static readonly string EVENT_TRAINING_R = "Training Right";
	public static readonly string EVENT_TRAINING_L = "Training Left";

	public static readonly string EVENT_TRAINING_CLEAR_N = "Neutral Training Data Cleared";
	public static readonly string EVENT_TRAINING_CLEAR_R = "Right Training Data Cleared";
	public static readonly string EVENT_TRAINING_CLEAR_L = "Left Training Data Cleared";

	public static readonly string EVENT_TRAINING_ACCEPT = "Training Data Accepted";
	public static readonly string EVENT_TRAINING_REJECT = "Training Data Rejected";

	public static readonly string EVENT_TRAINING_TRIAL_PASS_R = "Right Training Trial Passed";
	public static readonly string EVENT_TRAINING_TRIAL_PASS_L = "Left Training Trial Passed";

	public static readonly string EVENT_FAMI_START = "Start Familiarization";
    public static readonly string EVENT_FAMI_END = "Completed Familiarization";
    public static readonly string EVENT_FAMI_PROMT = "Trial Prompt Created";
    public static readonly string EVENT_FAMI_BLOCK_POS = "Block Position at Start of Navigation";
    public static readonly string EVENT_FAMI_PASS = "Familiarization Trial Passed";

	public static readonly string EVENT_BLOCK_ROTATE = "Block Rotated";
	public static readonly string EVENT_BLOCK_LEFT = "Block Moved Left";
	public static readonly string EVENT_BLOCK_RIGHT = "Block Moved Right";
    public static readonly string EVENT_BLOCK_CREATE = "Game Block Created";
    public static readonly string EVENT_BLOCK_DROP = "Game Block Dropped";

    public static readonly string EVENT_SCORE = "Score";
    public static readonly string EVENT_GAME_OVER = "Game Over";


//------------------------------Singleton Control Functions------------------------------//

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			this.CreateTitles();
			DontDestroyOnLoad(gameObject);
		}
		else if (instance != this)
		{
            Destroy(this.gameObject);
		}

	}

	public static LoggerCSV GetInstance()
	{
		return instance;
	}

//------------------------------CSV Functions------------------------------//

	private void CreateTitles()
	{
		string[] titles = { "External Time", "Internal Time", "Event", "AUX" };
		rows.Add(titles);
	}

    public void AddEvent(string event_log)
	{
        AddEvent(event_log, null);
	}

    public void AddEvent(string event_log, string aux){
        string[] toAdd = { DateTime.Now.ToString(), Time.time.ToString(), event_log, aux };
		rows.Add(toAdd);
    }

	public void PrintLogger()
	{
		for (int i = 0; i < rows.Count; i++)
		{
			string[] r = rows[i];
			string toPrint = "";
			for (int j = 0; j < r.Length; j++)
			{
				toPrint += r[j] + "    ";
			}
			Debug.Log("Row " + i.ToString() + ": " + toPrint);
		}
	}

    //Saves List<string> rows as a .csv file
	public void SaveCSV()
	{
		string[][] output = new string[rows.Count][];
		for (int i = 0; i < output.Length; i++)
		{
			output[i] = rows[i];
		}
		int len = output.GetLength(0);
		string divider = ",";

		StringBuilder sb = new StringBuilder();

		for (int index = 0; index < len; index++)
			sb.AppendLine(string.Join(divider, output[index]));


		string filePath = getPath();

		StreamWriter outStream = System.IO.File.CreateText(filePath);
		outStream.WriteLine(sb);
		outStream.Close();

        rows = new List<string[]>();
        CreateTitles();
	}

	// Following method is used to retrive the relative path as device platform
	private string getPath()
	{
		string mode;
		if (gameMode == NORMAL_MODE)
			mode = "_Normal";
		else
			mode = "_BCI";

        string final = Application.persistentDataPath;
        if (final.EndsWith("brain_blocks"))
            final = final.Substring(0, final.Length-12);
        return final + participantID.ToString() + mode + "_BrainBlocks" + ".csv";
	}
	
}
