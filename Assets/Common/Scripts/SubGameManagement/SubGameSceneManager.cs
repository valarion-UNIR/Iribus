using Eflatun.SceneReference;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;

public static class SubGameSceneManager
{
    public static void LoadScene(SubGame subGame, SceneReference scene, SceneReference loadingScene = null, LocalPhysicsMode physics = LocalPhysicsMode.None)
        => SubGameScreenManager.instance[subGame].LoadScene(scene, loadingScene, physics);
}
