using System;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }

    private const string ResourcesFolder = "Dialogos";

    private Dictionary<string, Dictionary<string, string>> _translations;
    private string _currentLangCode = "en";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SetLanguage(GetLanguageCode(Application.systemLanguage));
    }

    private void LoadTranslations()
    {
        var path = "AllDialogues";
        TextAsset csvAsset = Resources.Load<TextAsset>(path);
        if (csvAsset == null)
        {
            Debug.LogError($"Falta el CSV en /{path}.csv");
            _translations = new Dictionary<string, Dictionary<string, string>>();
            return;
        }

        _translations = ParseCsv(csvAsset.text);
        Debug.Log($"Cargadas {_translations.Count} traducciones.");
    }

    public void SetLanguage(string langCode)
    {
        langCode = langCode.ToLowerInvariant();
        if (string.Equals(_currentLangCode, langCode, StringComparison.OrdinalIgnoreCase))
            return;

        _currentLangCode = langCode;

        if (_translations == null)
            LoadTranslations();

        Debug.Log($"[DialogueManager] Cambio de idioma '{_currentLangCode}'");
    }

    private string GetLanguageCode(SystemLanguage lang)
    {
        switch (lang)
        {
            case SystemLanguage.Spanish: return "es";
            case SystemLanguage.French: return "eus";
            case SystemLanguage.German: return "cat";
            default: return "en";
        }
    }

    private Dictionary<string, Dictionary<string, string>> ParseCsv(string csvText)
    {
        var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        var lines = csvText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
            return result;

        var headers = lines[0].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        var langCodes = new List<string>();
        foreach (var h in headers)
            langCodes.Add(h.Trim().ToLowerInvariant());

        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(new[] { ';' }, StringSplitOptions.None);
            if (parts.Length != langCodes.Count)
                continue;

            var key = parts[0].Trim();
            if (string.IsNullOrEmpty(key))
                continue;

            var translationMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int j = 0; j < langCodes.Count; j++)
            {
                translationMap[langCodes[j]] = parts[j].Trim();
            }

            result[key] = translationMap;
        }

        return result;
    }

    public string GetDialogue(string englishText)
    {
        if (_translations != null &&
            _translations.TryGetValue(englishText, out var transMap) &&
            transMap.TryGetValue(_currentLangCode, out var text) &&
            !string.IsNullOrEmpty(text))
        {
            return text;
        }

        Debug.LogWarning($"Falta traduccion de '{englishText}' para '{_currentLangCode}'");
        return englishText;
    }
}
