using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //Variables and Objects
    public static Group activeGroup = null;
    public static GameObject holdGroup = null;
    private static GameObject nextGroup = null;
    public GameObject[] groups;
    public static Transform origin = null;
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnNext();
        origin = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnNext()
    {
        //Random next
        System.Random random = new System.Random();

        if (activeGroup == null)
        {
            //Make first active group
            GameObject newGroup = Instantiate(groups[random.Next(0, groups.Length)], transform.position, Quaternion.identity);
            activeGroup = newGroup.GetComponent<Group>();
            activeGroup.isActive = true;

            //Make first next group
            nextGroup = Instantiate(groups[random.Next(0, groups.Length)], GameObject.Find("pnlNext").transform.position - new Vector3(0.0f, 0,0f), Quaternion.identity);
            nextGroup.transform.localScale = new Vector3(0.6f, 0.6f);
        }
        else
        {
            //Change next group to current group
            nextGroup.transform.localScale = new Vector3(1.0f, 1.0f);
            nextGroup.transform.position = transform.position;
            activeGroup = activeGroup = nextGroup.GetComponent<Group>();
            activeGroup.isActive = true;

            //Make new next group
            nextGroup = Instantiate(groups[random.Next(0, groups.Length)], GameObject.Find("pnlNext").transform.position - new Vector3(0.0f, 0.0f), Quaternion.identity);
            nextGroup.transform.localScale = new Vector3(0.6f, 0.6f);
        }
        
    }
}
