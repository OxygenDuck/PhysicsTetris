using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //PhysicsTetris
    //Programming by Peter Janssen
    //5 november 2019
    //Variables and Objects
    public static Group activeGroup = null;
    public static GameObject holdGroup = null;
    private static GameObject nextGroup = null;
    public GameObject[] groups;
    public static Transform origin = null;
    public static bool spawnedThisFrame = false;
    public static float lastSpawn;

    // Start is called before the first frame update
    void Start()
    {
        SpawnNext();
        origin = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastSpawn >= 1)
        {
            spawnedThisFrame = false;
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    //Spawn the next group of blocks
    public void SpawnNext()
    {
        //C# rng
        System.Random random = new System.Random();

        if (!spawnedThisFrame && !Playfield.gameOver)
        {
            if (activeGroup == null)
            {
                //Make first active group
                GameObject newGroup = Instantiate(groups[random.Next(0, groups.Length)], transform.position, Quaternion.identity);
                newGroup.transform.localScale = new Vector3(0.9f, 0.9f);
                activeGroup = newGroup.GetComponent<Group>();
                activeGroup.SetActive();

                //Make first next group
                nextGroup = Instantiate(groups[random.Next(0, groups.Length)], GameObject.Find("pnlNext").transform.position - new Vector3(0.0f, 0, 0f), Quaternion.identity);
                nextGroup.transform.localScale = new Vector3(0.6f, 0.6f);
                spawnedThisFrame = true;
            }
            else
            {
                //Change next group to current group
                nextGroup.transform.localScale = new Vector3(0.9f, 0.9f);
                nextGroup.transform.position = transform.position;
                activeGroup = nextGroup.GetComponent<Group>();
                activeGroup.SetActive();

                //Make new next group
                nextGroup = Instantiate(groups[random.Next(0, groups.Length)], GameObject.Find("pnlNext").transform.position - new Vector3(0.0f, 0.0f), Quaternion.identity);
                nextGroup.transform.localScale = new Vector3(0.6f, 0.6f);
                spawnedThisFrame = true;
            }
            lastSpawn = Time.time;
        }
    }
}
