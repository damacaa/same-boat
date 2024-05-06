using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedText : MonoBehaviour
    {
        public static Language DEFAULT_LANGUAGE = Language.En;

        [SerializeField]
        TextMeshProUGUI _text;

        [SerializeField]
        private string[] _texts;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();

            LocalizationManager.OnLanguageChanged += HandleNewLanguage;
            //ForceUpdate();
            //Debug.Log(GetCurrentText());
        }

        private void HandleNewLanguage(Language language)
        {
            _text.text = _texts[(int)language];
        }

        public void SetText(Language language, string text)
        {
            _texts[(int)language] = text;

#if UNITY_EDITOR
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

        public void ForceUpdate()
        {
            HandleNewLanguage(LocalizationManager.CurrentLanguage);
        }

        private void OnEnable()
        {
            ForceUpdate();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            if (!_text)
                _text = GetComponent<TextMeshProUGUI>();


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

            int id = (int)DEFAULT_LANGUAGE;
            if (string.IsNullOrEmpty(_texts[id]))
                _texts[id] = _text.text;
            else
                _text.text = _texts[id];
        }
#endif
    }
}
