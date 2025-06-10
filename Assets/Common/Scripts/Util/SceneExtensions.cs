using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public static class SceneExtensions
{
    public static IEnumerable<T> GetSceneComponents<T>(this Scene scene, bool includeInactive = true)
        => scene.GetRootGameObjects().SelectMany(c => c.GetComponents<T>().Concat(c.GetComponentsInChildren<T>(includeInactive)));
}