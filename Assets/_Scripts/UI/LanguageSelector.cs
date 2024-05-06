using Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LanguageSelector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Animator _animator;
    private Localization.Language _language;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (_language)
        {
            case Localization.Language.En:
                _language = Localization.Language.Es;
                _animator.Play("Right");
                break;
            case Localization.Language.Es:
                _language = Localization.Language.En;
                _animator.Play("Left");
                break;
        }

        LocalizationManager.SetLanguage(_language);
    }

    private void OnEnable()
    {
        if (LocalizationManager.CurrentLanguage == Localization.Language.Es)
        {
            _language = Localization.Language.Es;
            _animator.Play("Right");
        }
    }
}
