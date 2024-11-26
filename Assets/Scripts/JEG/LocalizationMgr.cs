using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageChangeButton : MonoBehaviour
{
    private void Start()
    {
        if (LanguageSingleton.Instance.language == "ko")
        {
            SetLanguage("ko");
        } else if (LanguageSingleton.Instance.language == "en")
        {
            SetLanguage("en");
        } else if (LanguageSingleton.Instance.language == "zh")
        {
        SetLanguage("zh-Hans");
        }
    }

    private void SetLanguage(string languageCode)
    {
        var locale = LocalizationSettings.AvailableLocales.Locales.Find(l => l.Identifier.Code == languageCode);
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
        else
        {
            Debug.LogError($"Locale not found: {languageCode}");
        }
    }
}
