using Eflatun.SceneReference;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;

public static class SubGameSceneManager
{
    public static Awaitable LoadScene(SubGame subGame, SceneReference scene, SceneReference loadingScene = null, LocalPhysicsMode physics = LocalPhysicsMode.None)
    {
        var screen = SubGameScreenManager.instance[subGame];
        if (screen != null)
            return SubGameScreenManager.instance[subGame].LoadScene(scene, loadingScene, physics);
        else
            return SubGameScreen.LoadSceneStatic(scene, loadingScene, physics);
    }
}
