using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Playfield : MonoBehaviour
{
    //PhysicsTetris
    //Programming by Peter Janssen
    //5 november 2019

    //Variables
    public static int w = 10;
    public static int h = 20;
    public static int score = 0;
    public static int level = 1;
    public static float lastSwap = 0;
    public static bool gameOver = false;
    public static Transform[,] grid = new Transform[w, h];
    public static List<GameObject> gridItems = new List<GameObject>();

    //Update score and level label
    private static void UpdateLabels()
    {
        Text CurrentLabel = GameObject.Find("lblScore").GetComponent<Text>();
        CurrentLabel.text = "Score: " + score.ToString();
        CurrentLabel = GameObject.Find("lblLevel").GetComponent<Text>();
        CurrentLabel.text = "Level: " + level.ToString();
    }

    //Increase score and level
    public static void IncreaseScore(int Score)
    {
        score += Score;
        level++;
    }

    //Calculate speed for falling groups
    public static float CalculateLevelSpeed()
    {
        if (level >= 20)
        {
            return 1.5f;
        }
        return 0.5f + (0.05f * level) - 0.05f;
    }

    //Round Vector2 classes
    public static Vector2 RoundVec2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    //Check Inside border
    public static bool InsideBorder(Vector2 pos)
    {
        return ((int)pos.x >= 0 &&
            (int)pos.x < w &&
            (int)pos.y >= 0);
    }

    //Delete Row
    public static void DeleteRow(int y)
    {
        for (int x = 0; x < w; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    //Decrease row
    public static void DecreaseRow(int y)
    {
        for (int x = 0; x < w; x++)
        {
            if (grid[x, y] != null)
            {
                // Move one towards bottom
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                // Update Block position
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    //Lower all rows above the given row
    public static void DecreaseRowsAbove(int y)
    {
        for (int i = y; i < h; i++)
        {
            DecreaseRow(i);
        }
    }

    //Check if row is full
    public static bool IsRowFull(int y)
    {
        for (int x = 0; x < w; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }

    //Delete full rows
    public static void DeleteFullRows()
    {
        int addScore = 0;
        for (int y = 0; y < h; y++)
        {
            if (IsRowFull(y))
            {
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                addScore++;
                y--;
            }
        }
        if (addScore > 0)
        {
            IncreaseScore(addScore);
        }

        UpdateLabels();

        //DEBUG show position of grid items
        foreach (GameObject item in gridItems)
        {
            Destroy(item);
        }
        gridItems.Clear();

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (grid[x, y] != null)
                {
                    gridItems.Add((GameObject)Instantiate(Resources.Load("debugBlock"), new Vector3(x, y, -1), new Quaternion()));
                }
            }
        }
    }

    //Show a game over screen
    public static void ShowGameOver()
    {
        Instantiate(Resources.Load("gameOverScreen"), GameObject.Find("Canvas").transform);
    }
    
}
