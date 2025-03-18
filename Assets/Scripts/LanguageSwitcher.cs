using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;
using System.Globalization;
using UnityEngine.Localization;

public class LanguageSwitcher : MonoBehaviour
{
    private Button languageButton;
    private int _languageIndex; 
    
    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        
        languageButton = root.Q<Button>("LanguageButton");
        languageButton.clicked += OnLanguageButtonClicked;
        Load(); 
    }

    private void OnDisable()
    {
        languageButton.clicked -= OnLanguageButtonClicked;
    }

    private void OnLanguageButtonClicked()
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        var currentIndex = locales.IndexOf(LocalizationSettings.SelectedLocale);
        _languageIndex = (currentIndex + 1) % locales.Count;
        LocalizationSettings.SelectedLocale = locales[_languageIndex];
        Save(); 
    }
    
    private void Save()
    {
        PlayerPrefs.SetFloat("Language", _languageIndex);
        PlayerPrefs.Save(); 
    }

    private void Load()
    {
        _languageIndex = PlayerPrefs.GetInt("Language", _languageIndex);
    }
}