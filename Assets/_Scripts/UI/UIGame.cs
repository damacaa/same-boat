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

    [Header("Rules")]
    [SerializeField]
    GameObject _rulePrefab;
    [SerializeField]
    GameObject _ruleList;
    [SerializeField]
    Sprite[] _ruleSprites;

    [Header("ExtraInfo")]

    [SerializeField]
    GameObject _infoPrefab;
    [SerializeField]
    GameObject _infoList;

    Dictionary<Rule, GameObject> _rules = new();
    Dictionary<TransportableSO, GameObject> _infos = new();

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
            g.transform.localScale = Vector3.one;
            RuleIcon r = g.GetComponent<RuleIcon>();

            var imageB = r.B.GetComponent<Image>();
            imageB.sprite = rule.B.icon;
            //imageB.SetNativeSize();

            var imageA = r.A.GetComponent<Image>();
            //imageA.preserveAspect = true;
            imageA.sprite = rule.A.icon;
            //imageA.SetNativeSize();

            var imageA2 = r.A2.GetComponent<Image>();

            var icon = r.Icon.GetComponent<Image>();


            switch (rule.comparison)
            {
                case Rule.RuleType.CantCoexist:
                    icon.sprite = _ruleSprites[0];
                    break;
                case Rule.RuleType.CountMustBeGreaterThan:
                    icon.sprite = _ruleSprites[1];

                    imageA2.sprite = rule.A.icon;
                    imageA2.gameObject.SetActive(true);
                    //imageA2.SetNativeSize();
                    break;
                case Rule.RuleType.CountMustBeGreaterEqualThan:
                    icon.sprite = _ruleSprites[2];

                    imageA2.sprite = rule.A.icon;
                    imageA2.gameObject.SetActive(true);
                    //imageA2.SetNativeSize();
                    break;
                case Rule.RuleType.Requires:
                    icon.sprite = _ruleSprites[3];
                    break;
            }

            _rules.Add(rule, g);
        }

        foreach (var g in _infos.Values)
        {
            g.transform.SetParent(null);
            Destroy(g);
        }
        _infos.Clear();

        bool showWeight = level.BoatMaxWeightAllowed > 0;
        bool showTravelCost = level.BoatMaxTravelCost > 0;

        if (showWeight || showTravelCost)
        {
            _infoList.SetActive(true);

            HashSet<TransportableSO> list = new();

            foreach (var island in level.Islands)
            {

                foreach (var item in island.transportables)
                {
                    if (item.name == "Man")
                        continue;

                    if (list.Contains(item))
                        continue;

                    list.Add(item);

                    GameObject g = GameObject.Instantiate(_infoPrefab);
                    g.transform.SetParent(_infoList.transform);

                    g.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = item.icon;

                    if (showWeight)
                    {
                        g.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = item.Weight.ToString() + "kg";

                    }
                    else if (showTravelCost)
                    {
                        g.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = item.Weight.ToString() + "min";

                    }

                    _infos.Add(item, g);

                    g.transform.localScale = Vector3.one;
                }
            }
        }
        else
        {
            _infoList.SetActive(false);
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
