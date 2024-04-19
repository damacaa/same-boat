using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    [Header("Game UI elements")]
    [SerializeField]
    TextMeshProUGUI _descriptionText;
    [SerializeField]
    TextMeshProUGUI _crossingsCounterText;
    [SerializeField]
    GameObject _rulePrefab;
    [SerializeField]
    GameObject _ruleList;
    [SerializeField]
    Sprite[] _ruleSprites;

    Dictionary<Rule, GameObject> _rules = new();

    [Header("Screens")]
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

    internal void SetLevelDetails(Level level)
    {
        _descriptionText.text = $"{level.name}:\n{level}";


        foreach (var g in _rules.Values)
        {
            g.transform.SetParent(null);
            Destroy(g);
        }
        _rules.Clear();

        foreach (var rule in level.Rules)
        {
            GameObject g = GameObject.Instantiate(_rulePrefab);
            g.transform.SetParent(_ruleList.transform);

            g.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = rule.B.sprite;
            
            g.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = rule.A.sprite;

            switch (rule.comparison)
            {
                case Rule.RuleType.CantCoexist:
                    g.transform.GetChild(3).gameObject.GetComponent<Image>().sprite = _ruleSprites[0];
                    break;
                case Rule.RuleType.CountMustBeGreaterThan:
                    g.transform.GetChild(3).gameObject.GetComponent<Image>().sprite = _ruleSprites[1];
                    g.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = rule.A.sprite;
                    g.transform.GetChild(1).gameObject.SetActive(true);
                    break;
                case Rule.RuleType.CountMustBeGreaterEqualThan:
                    g.transform.GetChild(3).gameObject.GetComponent<Image>().sprite = _ruleSprites[2];
                    g.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = rule.A.sprite;
                    g.transform.GetChild(1).gameObject.SetActive(true);
                    break;
                case Rule.RuleType.Requires:
                    g.transform.GetChild(3).gameObject.GetComponent<Image>().sprite = _ruleSprites[3];
                    break;
            }

            _rules.Add(rule, g);
        }
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
