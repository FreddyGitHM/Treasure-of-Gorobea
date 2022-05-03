using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeID : MonoBehaviour
{
    private int treeID;

    public void setTreeID(int treeID)
    {
        this.treeID = treeID;
    }

    public int getTreeID()
    {
        return treeID;
    }
}
