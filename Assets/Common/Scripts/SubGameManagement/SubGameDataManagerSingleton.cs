using System.Collections.ObjectModel;
using UnityEngine;

public class SubGameDataManagerSingleton : MonoBehaviour
{
    [SerializeField] private SubGameDataManager manager;

    private static SubGameDataManagerSingleton instance;

    public static ReadOnlyDictionary<SubGame, SubGameData> Data => instance.manager.Data;

    private void Awake()
    {
        instance = this;
    }
}
