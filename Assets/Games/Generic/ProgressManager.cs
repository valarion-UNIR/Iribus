using AutoTiling;
using System.IO;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;
    private ProgressData progressData = new ProgressData();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadData();
    }

    public void SaveFile()
    {
        var json = JsonUtility.ToJson(progressData);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "progressSave.json"), json);
    }

    private bool LoadData()
    {
        var path = Path.Combine(Application.persistentDataPath, "progressSave.json");
        if (File.Exists(path))
        {
            progressData = JsonUtility.FromJson<ProgressData>(File.ReadAllText(path));
            return true;
        }
        else
        {
            SaveFile();
            return false;
        }
    }

    private void DeleteData()
    {
        var path = Path.Combine(Application.persistentDataPath, "progressSave.json");
        if (!File.Exists(path)) { File.Delete(path); }
    }

    public void setMeleeUnlocked(bool isUnlocked)
    {
        progressData.meleeAAtack = isUnlocked;
        SaveFile();
    }

    public void setJoystickPicked(bool isUnlocked)
    {
        progressData.joystickPicked = isUnlocked;
        SaveFile();
    }

    public bool isMeleeUnlocked()
    {
        return progressData.meleeAAtack;
    }

    public bool isJoystickPicked()
    {   
        return progressData.joystickPicked;
    }

    public void addHealth(int health)
    {
        progressData.health += health;
    }

    public void addFirecrackers(int ammo)
    {
        progressData.firecrackers += ammo;
    }
}
