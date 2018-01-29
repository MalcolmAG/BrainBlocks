using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Set : MonoBehaviour {

    private void OnDrawGizmos()
    {
        Vector3 source = new Vector3(.5f, -.5f);
        Vector3 target = new Vector3(.5f, 15.5f);

        Gizmos.color = new Color(1, 1, 1, .2F);

        for (int i = 0; i < 9; i++){
            Gizmos.DrawLine(source, target);
            source.x += 1;
            target.x += 1;
        }

    }

    private bool orientation;

    private void Start()
    {
        orientation = true;
    }

    void Update()
	{
        if (orientation)
        {
            CheckRotate();
            CheckSnap();
        }
        else
        {
            CheckMoveLeft();
            CheckMoveRight();

            CheckFallDown();
            // Default position not valid? Then it's game over
            if (!LegalGridPos())
            {
                Debug.Log("GAME OVER");
                Destroy(gameObject);
            }
        }
	}

	void CheckRotate(){
		// Rotate
        if (Input.GetKeyDown(KeyCode.UpArrow))
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

    void CheckSnap(){
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            //snap to line
            orientation = false;
            
        }
    }

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



    void CheckFallDown(){
		// Fall
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
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

			// Disable script
			enabled = false;
		}
    }

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
	


}
