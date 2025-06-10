using System;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Pool;

public class SubGameAudioManager : AbstractSubGameObjectCreationManager<AudioSourcePool, SubGameAudioManager>
{
    //public static event Action<float> MuteAll;
    //public static event Action<float> ResumeAll;

    protected override AudioSourcePool CreateObject(SubGame subgame)
    {
        return new AudioSourcePool(subgame);
    }
}