using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Unity.VisualScripting.Icons;

namespace Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _text;

        [SerializeField]
        private string[] _texts;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();

            LocalizationManager.OnLanguageChanged += HandleNewLanguage;
            HandleNewLanguage(LocalizationManager.CurrentLanguage);
        }

        private void HandleNewLanguage(Language language)
        {
            _text.text = _texts[(int)language];
        }

        public void SetText(Language language, string text)
        {
            _texts[(int)language] = text;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                OnValidate();
#endif
        }

        public string GetText(Language language)
        {
            return _texts[(int)language];
        }

        public string GetCurrentText()
        {
            return _texts[(int)LocalizationManager.CurrentLanguage];
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            int availableLanguages = Math.Max(System.Enum.GetValues(typeof(Language)).Length, 1);
            if (_texts == null || _texts.Length != availableLanguages)
            {
                string[] newTexts = new string[availableLanguages];

                if (_texts != null)
                {
                    for (int i = 0; i < Mathf.Min(availableLanguages, _texts.Length); i++)
                    {
                        newTexts[i] = _texts[i];
                    }
                }

                _texts = newTexts;
            }

            if (!_text)
                _text = GetComponent<TextMeshProUGUI>();
            _text.text = _texts[0];
        }
#endif
    }
}
