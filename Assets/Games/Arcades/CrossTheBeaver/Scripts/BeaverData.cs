using System;
using UnityEngine;

[Serializable]
public class BeaverData
{
    public int levelNumber;
    //public int levelID;
    //public bool FirstPickUp;
    //public bool SecondPickUp;
    //public bool ThirdPickUp;

    public BeaverData(int levelNumber)
    {
        this.levelNumber = levelNumber;
        //this.levelID = levelID;
    }
}
