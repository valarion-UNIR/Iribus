using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private const string ResourcesFolder = "Dialogos";

    private Dictionary<string, Dictionary<int, string>> _dialogos;
    private string currentLangCode;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //CargarDialogosIdioma();
        SetLanguage(GetLanguageCode(Application.systemLanguage));
    }

    /// <summary>
    /// Carga y parsea el csv con el idioma del juego correspondiente
    /// </summary>
    private void CargarDialogosIdioma(string codeIdioma)
    {
        var pathDialogos = $"{ResourcesFolder}/{codeIdioma}_Dialogos";
        TextAsset csvDialogos = Resources.Load<TextAsset>(pathDialogos);

        if (csvDialogos == null)
        {
            Debug.LogError($"No se ha encontrado csv en Resources/{pathDialogos}.csv");
            _dialogos = new Dictionary<string, Dictionary<int, string>>();
            return;
        }

        _dialogos = ParseCsv(csvDialogos.text);
        Debug.Log($"Cargados {_dialogos.Count} personajes de '{codeIdioma}'.");
    }

    public void SetLanguage(string langCode)
    {
        if (string.Equals(currentLangCode, langCode, StringComparison.OrdinalIgnoreCase))
            return;

        currentLangCode = langCode;
        CargarDialogosIdioma(currentLangCode);
        Debug.Log($"[DialogueManager] Language switched to '{currentLangCode}'");
    }

    /// <summary>
    /// Para pillar los idiomas soportados. Igual se hace mejor de otra manera???
    /// </summary>
    private string GetLanguageCode(SystemLanguage lang)
    {
        switch (lang)
        {
            case SystemLanguage.Spanish: return "es";
            case SystemLanguage.French: return "fr";
            case SystemLanguage.German: return "de";
            default: return "en";
        }
    }

    /// <summary>
    /// Parsea el CSV al diccionario de dialogos, tiene que tener personaje, indice y el texto.
    /// </summary>
    private Dictionary<string, Dictionary<int, string>> ParseCsv(string csvText)
    {
        var result = new Dictionary<string, Dictionary<int, string>>(StringComparer.OrdinalIgnoreCase);

        var lines = csvText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // La primera linea es cabezera asi que se pasa
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(new[] { ';' }, 3);
            if (parts.Length < 3) continue;

            string character = parts[0].Trim();
            if (!int.TryParse(parts[1].Trim(), out int index)) continue;
            string text = parts[2].Trim();

            if (!result.TryGetValue(character, out var dict))
            {
                dict = new Dictionary<int, string>();
                result[character] = dict;
            }
            dict[index] = text;
        }

        return result;
    }

    /// <summary>
    /// Devuelve la linea de dialogo de ese personaje en ese indice
    /// </summary>
    public string GetDialogue(string character, int index)
    {
        if (_dialogos != null &&
            _dialogos.TryGetValue(character, out var dict) &&
            dict.TryGetValue(index, out var line))
        {
            return line;
        }
        Debug.LogWarning($"Sin dialogo para '{character}' en indice tal {index}");
        return null;
    }
}