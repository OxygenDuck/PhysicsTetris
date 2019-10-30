using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInput : MonoBehaviour
{
    public static Group group = null;

    void Start()
    {
        
    }

    void Update()
    {
        group = Spawner.activeGroup;
    }

    //Button Functions
    public void MoveLeft()
    {
        group.MoveLeft();
    }
    public void MoveRight()
    {
        group.MoveRight();
    }
    public void MoveDown()
    {
        group.MoveDown();
    }
    public void MoveRotate()
    {
        group.MoveRotate();
    }
    public void MoveToHold()
    {
        group.SwitchHold();
    }
}
