using NUnit.Framework.Constraints;
using System;
using UnityEngine;

[Serializable]
public class ProgressData
{
    [Header("Generic")]
    public bool joystickPicked;
    public int health;
    public int firecrackers;

    [Header("Iribus")]
    public bool meleeAAtack;
    public bool slingShot;
    public bool dash;
    public bool swim;

    public ProgressData()
    {
        joystickPicked = meleeAAtack = slingShot = dash = swim = false;
        health = firecrackers = 0;
    }
    public ProgressData(bool joystickPicked, bool meleeAAtack, bool slingShot, bool dash, bool swim, int health, int firecrackers)
    {
        this.joystickPicked = joystickPicked;
        this.meleeAAtack = meleeAAtack;
        this.slingShot = slingShot;
        this.dash = dash;
        this.swim = swim;
        this.health = health;
        this.firecrackers = firecrackers;
    }
}
