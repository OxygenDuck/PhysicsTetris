using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInput : MonoBehaviour
{
    public static Group group = null;

    //Button Functions
    public void MoveLeft()
    {
        group = Spawner.activeGroup;
        group.MoveLeft();
    }
    public void MoveRight()
    {
        group = Spawner.activeGroup;
        group.MoveRight();
    }
    public void MoveDown()
    {
        group = Spawner.activeGroup;
        group.MoveDown();
    }
    public void MoveRotate()
    {
        group = Spawner.activeGroup;
        group.MoveRotate();
    }
    public void MoveToHold()
    {
        group = Spawner.activeGroup;
        group.SwitchHold();
    }
}
