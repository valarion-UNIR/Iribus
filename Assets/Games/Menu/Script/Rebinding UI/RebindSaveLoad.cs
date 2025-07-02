using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.SceneManagement;

public class RebindSaveLoad : MonoBehaviour
{
    [SerializeField] private InputActionAsset actions;
    private string savedBindingsJson;

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        LoadInputSystemRebindings();
    }

    public bool HasUnsavedChanges()
    {
        string currentJson = actions.SaveBindingOverridesAsJson();
        return !string.Equals(savedBindingsJson, currentJson, StringComparison.Ordinal);
    }

    public void RevertChanges()
    {
        foreach (var map in actions.actionMaps)
        {
            map.LoadBindingOverridesFromJson(savedBindingsJson);
        }

        RebindActionUI[] rebindingUIs = GameObject.FindObjectsByType<RebindActionUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var ui in rebindingUIs)
        {
            ui.UpdateBindingDisplay();
        }
    }

    public void SaveInputSystemRebindings()
    {
        foreach (var map in actions.actionMaps)
        {
            string json = map.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString($"rebinds_{map.name}", json);
        }
        PlayerPrefs.Save();

        savedBindingsJson = actions.SaveBindingOverridesAsJson();
    }

    public void LoadInputSystemRebindings()
    {
        foreach (var map in actions.actionMaps)
        {
            string json = PlayerPrefs.GetString($"rebinds_{map.name}", string.Empty);
            if (!string.IsNullOrEmpty(json))
            {
                map.LoadBindingOverridesFromJson(json);
            }
        }

        savedBindingsJson = actions.SaveBindingOverridesAsJson();
    }
}
