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

    public static readonly string EVENT_RETRAIN = "Retrain Occured";
    public static readonly string EVENT_GAME_DROP = "Game Drop Time";
    public static readonly string EVENT_GAME_OVER = "Game Over/Score";
    public static readonly string EVENT_SCORE = "Final Score";


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
			Destroy(this);
		}

	}


	public static LoggerCSV GetInstance()
	{
		return instance;
	}

	private void CreateTitles()
	{
		string[] titles = { "Time Stamp", "Event", "Event Value" };
		rows.Add(titles);
	}

	public void AddEvent( string event_log, float event_val)
	{
        string[] toAdd = { DateTime.Now.ToString(), event_log, event_val.ToString() };
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
	public void SaveCSV()
	{
		Debug.Log("Saving CSV");
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
        Debug.Log(Application.persistentDataPath);
        return Application.persistentDataPath + "_"+ participantID.ToString() + mode + ".csv";
	}
	// Update is called once per frame
	void Update()
	{

	}
}
