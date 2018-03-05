using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Set : MonoBehaviour {

    public GameObject ghost;

    private bool orientation;

    private readonly float snapPos = 16f;

    private readonly float unSnapPos = 19f;

    private readonly Vector2 ghostStandByPos = Vector2.down * 10;

    public float runningTimer;

    private void Start(){
        runningTimer = Time.time;
        orientation = true;
        ghost = GameObject.Find(tag + "_ghost");
    }

    private void SwapGhosts(){
        ghost.transform.position = ghostStandByPos;
        if(orientation){
            ghost = GameObject.Find(tag + "_ghost");
        }
        else{
            ghost = GameObject.Find(tag + "_ghost_light");
        }
        UpdateGhost();
    }

    void Update(){
        Debug.Log("in update");
        if (!MainUIController.paused)
        {
            if (orientation)
            {
				Debug.Log("checking rotate");

				CheckRotate();
                CheckSnap();
            }
            else
            {
                CheckUnSnap();
                CheckMoveLeft();
                CheckMoveRight();

                CheckFallDown();

            }
            UpdateGhost();
        }

  	}

	//Positions block at the top of the game field
	void CheckSnap(){
        Debug.Log("Down arrow: " + Input.GetKeyDown(KeyCode.DownArrow));
        //Snap orientated group to top of play field
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            orientation = false;
            bool snap = true;
            while (snap){
                //Check if block is at the top
    			foreach (Transform child in transform)
    			{
                    if (Grid.ToGrid(child.position).y == snapPos)
    				{
    					snap = false;
    				}
    			}
                //Move down one and update if still snapping
                if (snap)
                {
                    transform.position += new Vector3(0, -1, 0);
                    UpdateGrid();
                }
            }
            SwapGhosts();
            
        }
    }

	//Positions block at the reorientation position
	void CheckUnSnap(){
        if(Input.GetKeyDown(KeyCode.UpArrow)){
            transform.position = new Vector2(transform.position.x, unSnapPos);
            orientation = true;
            SwapGhosts();
        }
    }

	//Listens for and applies drop action
	void CheckMoveLeft(){
		// Move Left
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			// Modify position
			transform.position += new Vector3(-1, 0, 0);

			// See if valid
			if (LegalGridPos())
				// Its valid. Update grid.
				UpdateGrid();
			else
				// Its not valid. revert.
				transform.position += new Vector3(1, 0, 0);
		}
    }

    //Listens for and applies rotate action
	void CheckRotate()
	{
		// Rotate
        if (Input.GetKeyDown(KeyCode.Space))
		{

			transform.Rotate(0, 0, -90);
			// See if valid
			if (LegalGridPos())
				// It's valid. Update grid.
				UpdateGrid();
			else
				// It's not valid. revert.
				transform.Rotate(0, 0, 90);
		}
	}


	//Listens for and applies move right action
	void CheckMoveRight(){
       // Move Right
        if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			// Modify position
			transform.position += new Vector3(1, 0, 0);

			// See if valid
			if (LegalGridPos())
				// It's valid. Update grid.
				UpdateGrid();
			else
				// It's not valid. revert.
				transform.position += new Vector3(-1, 0, 0);
		} 
    }

	//Listens for and applies drop action
	void CheckFallDown(){
		// Fall
        if (Input.GetKeyDown(KeyCode.DownArrow)){

            //Log Drop time in CSV file
            LogDrop();

			// Modify position
			transform.position += new Vector3(0, -1, 0);

			// See if valid
            while (LegalGridPos())
			{
				// It's valid. Update grid.
				UpdateGrid();
                transform.position += new Vector3(0, -1, 0);
			}
			// It's not valid. revert.
			transform.position += new Vector3(0, 1, 0);

			// Clear filled horizontal lines
			Grid.DeleteFullRows();

			// Spawn next Group
			FindObjectOfType<Spawn>().CreateNext();

            //Check Game Over
            CheckGameOver();

			// Disable script
			enabled = false;
		}
    }

	//Checks if positioning is allowed based on 2D array data structre
	bool LegalGridPos()
	{
		foreach (Transform child in transform)
		{
            Vector2 v = Grid.ToGrid(child.position);

			// Is the set leaving the playing field
            if (!Grid.InsideBorder(v))
				return false;

			// Block in grid cell (and not part of same group)?
			if (Grid.grid[(int)v.x, (int)v.y] != null &&
				Grid.grid[(int)v.x, (int)v.y].parent != transform)
				return false;
		}
		return true;
	}

	//Updates 2D array data structure with game object positions
    void UpdateGrid()
	{
		// Remove old children from grid
		for (int y = 0; y < Grid.h; ++y)
			for (int x = 0; x < Grid.w; ++x)
				if (Grid.grid[x, y] != null)
					if (Grid.grid[x, y].parent == transform)
						Grid.grid[x, y] = null;

		// Add new children to grid
		foreach (Transform child in transform)
		{
            Vector2 v = Grid.ToGrid(child.position);
			Grid.grid[(int)v.x, (int)v.y] = child;
		}
	}
	
    //Reorients and repositions ghost based on current block
	void UpdateGhost()
    {
        if (!enabled)
        {
            //Remove ghost
            ghost.transform.position = ghostStandByPos;
            return;
        }
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;

        bool dropping = true;
        while (dropping)
        {
            foreach (Transform child in ghost.transform)
            {
                Vector2 v = Grid.ToGrid(child.position);
                if (Grid.grid[(int)v.x, (int)v.y] != null &&
                    Grid.grid[(int)v.x, (int)v.y].parent != transform)
                {
                    dropping = false;
                    //Revert
                    ghost.transform.position += Vector3.up;
                }
                else if ((int)v.y == 0) dropping = false;
            }
            if (dropping)
                //Continue Dropping
                ghost.transform.position += Vector3.down;
        }
    }

    //Checks if block is above game area
    void CheckGameOver(){
        foreach (Transform child in transform){
            Vector2 v = Grid.ToGrid(child.position);
            if (v.y >= snapPos)
            {
                //Log Game over
				LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_GAME_OVER, MainUIController.score);

                MainUIController.score = 0;

                foreach(Transform c in transform.parent){
                    Destroy(c.gameObject);
                }

                //Restart Game
                FindObjectOfType<Spawn>().CreateFirst();
				FindObjectOfType<Spawn>().CreateNext();


				return;

			}
        }
    }

    //Logs drop time in csv file
    void LogDrop(){
		LoggerCSV.GetInstance().AddEvent(LoggerCSV.EVENT_GAME_DROP, Time.time - runningTimer);
        runningTimer = Time.time;
    }

}
