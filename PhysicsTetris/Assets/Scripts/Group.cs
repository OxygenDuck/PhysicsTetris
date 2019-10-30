using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    //Variables
    public bool isActive = false;
    // Time since last gravity tick
    float lastFall = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Default position not valid? Game over
        if (!IsValidGridPos() && isActive)
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (Input.GetKeyDown(KeyCode.Space) && (double)Time.time > (double)Playfield.lastSwap)
            {
                SwitchHold(); 
            }

            //Move Left
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveLeft();
            }

            //Move Right
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveRight();
            }

            //Rotate
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveRotate();
            }

            //Fall
            else if (Input.GetKeyDown(KeyCode.DownArrow) || (double)Time.time - (double)lastFall >= Playfield.CalculateLevelSpeed())
            {
                MoveDown();
            }
        }
    }

    bool IsValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = Playfield.RoundVec2(child.position);

            // Not inside Border?
            if (!Playfield.InsideBorder(v))
            {
                return false;
            }

            // Block in grid cell (and not part of same group)?
            if (Playfield.grid[(int)v.x, (int)v.y] != null && Playfield.grid[(int)v.x, (int)v.y].parent != transform)
            {
                return false;
            }
        }
        return true;
    }

    void UpdateGrid()
    {

        // Remove old children from grid
        for (int y = 0; y < Playfield.h; ++y)
        {
            for (int x = 0; x < Playfield.w; ++x)
            {
                if (Playfield.grid[x, y] != null)
                {
                    if (Playfield.grid[x, y].parent == transform)
                    {
                        Playfield.grid[x, y] = null;
                    }
                }
            }
        }
        if (isActive)
        {
            // Add new children to grid
            foreach (Transform child in transform)
            {
                Vector2 v = Playfield.RoundVec2(child.position);
                Playfield.grid[(int)v.x, (int)v.y] = child;
            }
        }
    }

    //Move functions
    public void MoveLeft()
    {
        //Modify position
        transform.position += new Vector3(-1, 0, 0);

        //Check position validity
        if (IsValidGridPos())
        {
            //Valid
            UpdateGrid();
        }
        else
        {
            //Invalid
            transform.position += new Vector3(1, 0, 0);
        }
    }

    public void MoveRight()
    {
        //Modify position
        transform.position += new Vector3(1, 0, 0);

        //Check position validity
        if (IsValidGridPos())
        {
            //Valid
            UpdateGrid();
        }
        else
        {
            //Invalid
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    public void MoveDown()
    {
        //Modify position
        transform.position += new Vector3(0, -1, 0);

        //See if valid
        if (IsValidGridPos())
        {
            //It's valid. Update grid.
            UpdateGrid();
        }
        else
        {
            //It's not valid. revert.
            transform.position += new Vector3(0, 1, 0);

            //Clear filled horizontal lines
            Playfield.DeleteFullRows();

            //Spawn next Group
            FindObjectOfType<Spawner>().SpawnNext();

            //Disable script
            enabled = false;
        }
        lastFall = Time.time;
    }

    public void MoveRotate()
    {
        transform.Rotate(0, 0, -90);

        //Check position validity
        if (IsValidGridPos())
        {
            //Valid
            UpdateGrid();
        }
        else
        {
            //Invalid
            transform.Rotate(0, 0, 90);
        }
    }

    public void SwitchHold()
    {
        //Set to hold
        isActive = false;
        UpdateGrid(); //Empty current spaces on grid
        if (Spawner.holdGroup == null)
        {
            Spawner.holdGroup = gameObject;
            transform.position = GameObject.Find("pnlHold").transform.position;
            transform.localScale = new Vector3(0.6f, 0.6f);
            FindObjectOfType<Spawner>().SpawnNext();
        }
        else
        {
            //set old hold to start position and make it active
            GameObject oldHold = Spawner.holdGroup;
            oldHold.transform.position = Spawner.origin.position;
            oldHold.transform.localScale = new Vector3(1.0f, 1.0f);
            oldHold.GetComponent<Group>().isActive = true;

            Spawner.holdGroup = gameObject;
            transform.position = GameObject.Find("pnlHold").transform.position;
            transform.localScale = new Vector3(0.6f, 0.6f);
        }
        Playfield.lastSwap = Time.time;
    }
}
