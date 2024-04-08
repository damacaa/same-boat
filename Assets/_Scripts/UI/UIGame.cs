using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGame : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _descriptionText;
    [SerializeField]
    TextMeshProUGUI _crossingsCounterText;

    [SerializeField]
    GameObject _hud;
    [SerializeField]
    GameObject _winScreen;
    [SerializeField]
    GameObject _failScreen;
    [SerializeField]
    GameObject _loadingScreen;

    public enum UIState
    {
        Playing,
        Win,
        Fail
    }

    internal void SetCrossings(int crossings)
    {
        _crossingsCounterText.text = crossings.ToString();
    }

    internal void SetDescription(string levelDescription)
    {
        _descriptionText.text = levelDescription;
    }

    public void SetState(UIState state)
    {
        switch (state)
        {
            case UIState.Playing:
                _hud.SetActive(true);
                _winScreen.SetActive(false);
                _failScreen.SetActive(false);
                InputSystem.InputEnabled = true;
                break;
            case UIState.Win:
                _hud.SetActive(false);
                _winScreen.SetActive(true);
                _failScreen.SetActive(false);
                InputSystem.InputEnabled = false;
                break;
            case UIState.Fail:
                _hud.SetActive(false);
                _winScreen.SetActive(false);
                _failScreen.SetActive(true);
                InputSystem.InputEnabled = false;
                break;
        }
    }

    public void ShowLoadingScreen()
    {
        _loadingScreen.SetActive(true);
        var animator = _loadingScreen.GetComponentInChildren<Animator>();
        animator.SetTrigger("StartAnimation");
    }

    public void HideLoadingScreen()
    {
        //_loadingScreen.SetActive(false);
        var animator = _loadingScreen.GetComponentInChildren<Animator>();
        animator.SetTrigger("EndAnimation");
        StartCoroutine(C());
    }

    IEnumerator C()
    {
        yield return new WaitForSeconds(0.05f);
        _loadingScreen.SetActive(false);
        yield return null;  
    }
}
