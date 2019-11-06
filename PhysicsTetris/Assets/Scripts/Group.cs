using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    //PhysicsTetris
    //Programming by Peter Janssen
    //5 november 2019

    //Variables
    public bool isActive = false;
    bool bounce = false;
    public Rigidbody2D physics; //Assigned in inspector
    float timeBuffer = 0; //Set to check for difference in time
    Vector3 prevPosition;

    // Start is called before the first frame update
    void Start()
    {
        timeBuffer = Time.time;
        prevPosition = transform.position;
    }

    //Check if a game over is triggered
    private void CheckForGameOver()
    {
        if (!IsValidGridPos() && isActive && transform.position == GameObject.Find("Spawner").transform.position)
        {
            Debug.Log("GAME OVER");
            Playfield.gameOver = true;
            Playfield.ShowGameOver();
            Destroy(gameObject);

            //Hide UI Elements
            GameObject.Find("pnlHold").SetActive(false);
            GameObject.Find("pnlNext").SetActive(false);
            GameObject.Find("Walls").SetActive(false);
            foreach (Group group in FindObjectsOfType<Group>())
            {
                group.gameObject.SetActive(false);
            }
            foreach (GameObject item in Playfield.gridItems)
            {
                Destroy(item);
            }
            Playfield.gridItems.Clear();
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

            //increase fall speed
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveDown();
            }

            //Land
            else if (IsPositionOutOfBounds() || ((Time.time - timeBuffer >= 2) && physics.velocity.x == 0 && physics.velocity.y == 0 && !Spawner.spawnedThisFrame))
            {
                Land();
            }

            //Land if position doesn't change
            if (Time.time - timeBuffer >= 0.2f)
            {
                if (prevPosition == transform.position)
                {
                    Land();
                }
                timeBuffer = Time.time;
                prevPosition = transform.position;
            }

            //Change velocity if it exceeds the maximum
            if (physics.velocity.y < -25)
            {
                physics.velocity = new Vector2(physics.velocity.x, -25);
            }

            //set bounciness
            SetBounciness();

            //Check for game over
            CheckForGameOver();
        }
    }

    //Set bounciness of object
    private void SetBounciness()
    {
        if (bounce)
        {
            Resources.Load<PhysicsMaterial2D>("Bounce").bounciness = 0.3f;
        }
        else
        {
            Resources.Load<PhysicsMaterial2D>("Bounce").bounciness = 0;
        }
        
        physics.sharedMaterial = Resources.Load<PhysicsMaterial2D>("Bounce");
    }


    //Check for out of bounds position
    private bool IsPositionOutOfBounds()
    {
        foreach (Transform child in transform)
        {
            if (child.position.y < -0.5f)
            {
                return true;
            }
        }
        if (OverlappingBlocks() > 1)
        {
            return true;
        }
        return false;
    }

    //Calculate overlapping blocks
    private int OverlappingBlocks()
    {
        int overlapping = 0;

        foreach (Transform child in transform)
        {
            int x = Mathf.RoundToInt(child.position.x);
            int y = Mathf.RoundToInt(child.position.y);

            if (Playfield.grid[x, y] != null)
            {
                overlapping++;
            }
        }
        return overlapping;
    }

    //Land the group of blocks
    private void Land()
    {
        //Transform the position to a static form
        transform.localScale = new Vector3(1.0f, 1.0f);
        transform.position = Playfield.RoundVec2(transform.position);

        //get nearest 90 degrees and rotate perfectly
        var degrees = Mathf.Round(transform.localEulerAngles.z / 90) * 90;
        transform.rotation = Quaternion.Euler(0, 0, degrees);
        SetInactive();

        //set to valid coordinates
        SetValidPosition();

        //remove physics
        Destroy(physics);

        //Clear filled horizontal lines
        UpdateGrid();
        Playfield.DeleteFullRows();

        //Spawn next Group
        FindObjectOfType<Spawner>().SpawnNext();

        //Disable script for this group
        enabled = false;
    }

    //Set a valid position for the group
    private void SetValidPosition()
    {
        for (int y = 0; y < Playfield.h; y++)
        {
            if (transform.position.y > 1) //check if the group is able to go down 1 row
            {
                transform.position += new Vector3(0, -1);
                if (!IsValidGridPos()) //if invalid, revert
                {
                    transform.position += new Vector3(0, 1);
                }
            }
        }
        
        if (IsValidGridPos()) //return if position is already valid
        {
            return;
        }

        //Set position to above the floor
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).position.y < 0)
            {
                transform.position += new Vector3(0, 1);
                i = 0;
            }
        }

        //If a valid position is already set, return
        if (IsValidGridPos())
        {
            return;
        }

        //check left and right for open space
        transform.position += new Vector3(-1, 0);
        if (IsValidGridPos())
        {
            return;
        }
        transform.position += new Vector3(2, 0);
        if (IsValidGridPos())
        {
            return;
        }
        transform.position += new Vector3(-1, 0);

        //if a valid grid position still has not been found, go 1 higher and search again
        transform.position += new Vector3(0, 1);
        SetValidPosition();
    }

    //Set the current group inactive
    public void SetInactive()
    {
        isActive = false;
        physics.gravityScale = 0;
        physics.velocity = new Vector2(0, 0);
    }

    //Set the current group active
    public void SetActive()
    {
        isActive = true;
        physics.gravityScale = Playfield.CalculateLevelSpeed();
        CheckForGameOver();
    }

    //See if the position is valid
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

    //Update the grid
    void UpdateGrid()
    {
        // Remove old children from grid
        RemoveGridChildren();
        // Add new children to grid
        if (IsValidGridPos()) //prevent adding children if invalid position
        {
            foreach (Transform child in transform)
            {
                Vector2 v = Playfield.RoundVec2(child.position);
                Playfield.grid[(int)v.x, (int)v.y] = child;
            }
        }
    }

    //Remove old children from the grid
    private void RemoveGridChildren()
    {
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
    }

    //Mobile Move functions
    public void MoveLeft()
    {
        if (physics.velocity.y <= -1 || physics.velocity.y >= 1)
        {
            //Modify velocity
            physics.velocity += new Vector2(-5, 0);
        }
    }

    public void MoveRight()
    {
        if (physics.velocity.y <= -1 || physics.velocity.y >= 1)
        {
            //Modify velocity
            physics.velocity += new Vector2(5, 0);
        }
    }

    public void MoveDown()
    {
        if (physics.velocity.y <= -1)
        {
            //Modify velocity
            physics.velocity *= new Vector2(1, 2);
        }
        else if (physics.velocity.y >= 1)
        {
            physics.velocity *= -1;
        }

        bounce = true;
    }

    public void MoveRotate()
    {
        transform.Rotate(0, 0, -90);

        //Check position validity
        if (!IsValidGridPos())
        {
            //Invalid
            transform.Rotate(0, 0, 90);
        }
    }

    //Switch the groups in the hold
    public void SwitchHold()
    {
        SetInactive();
        //Empty current spaces on grid
        UpdateGrid();
        RemoveGridChildren();

        if (Spawner.holdGroup == null)
        {
            SetToHold();
            Spawner.spawnedThisFrame = false; //Allow for new spawn
            FindObjectOfType<Spawner>().SpawnNext();
        }
        else
        {
            //set old hold to start position and make it active
            GameObject oldHold = Spawner.holdGroup;
            oldHold.transform.position = Spawner.origin.position;
            oldHold.transform.localScale = new Vector3(0.9f, 0.9f);
            Spawner.spawnedThisFrame = true;
            Spawner.lastSpawn = Time.time;
            Spawner.activeGroup = oldHold.GetComponent<Group>();
            oldHold.GetComponent<Group>().physics.WakeUp();
            oldHold.GetComponent<Group>().physics.freezeRotation = false;
            oldHold.GetComponent<Group>().SetActive();

            SetToHold();
        }
        Playfield.lastSwap = Time.time;
    }

    //Set to hold
    private void SetToHold()
    {
        Spawner.holdGroup = gameObject;
        transform.position = GameObject.Find("pnlHold").transform.position;
        transform.localScale = new Vector3(0.6f, 0.6f);
        transform.rotation = new Quaternion(0, 0, 0, 0);
        physics.freezeRotation = true;
        physics.Sleep();
        bounce = false;
    }
}
