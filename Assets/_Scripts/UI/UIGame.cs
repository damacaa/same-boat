using Solver;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    private const string NUMBER_VALUE = "{N}";

    public enum UIState
    {
        Playing,
        Win,
        Fail
    }

    [SerializeField]
    private UIState _state;
    [Space]


    [Header("Game UI elements")]
    [SerializeField]
    TextMeshProUGUI _descriptionText;

    [SerializeField]
    TextMeshProUGUI _crossingsCounterText;

    // Level info and rules
    [SerializeField]
    TextMeshProUGUI _weigthText;
    [SerializeField]
    TextMeshProUGUI _travelTimeText;

    int _maxWeight;
    int _maxTime;



    // Rules
    [Header("Rules")]
    [SerializeField]
    GameObject _rulePrefab;  
    [SerializeField]
    GameObject _ruleSeparatorPrefab;
    [SerializeField]
    GameObject _ruleList;
    [SerializeField]
    Sprite[] _ruleSprites;

    Dictionary<Rule, GameObject> _rules = new();

    // Character info
    [Header("CharacterInfo")]
    [SerializeField]
    GameObject _infoPrefab;
    [SerializeField]
    GameObject _infoList;

    Dictionary<TransportableSO, GameObject> _infos = new();

    // States
    [Header("Screens")]
    [SerializeField]
    GameObject _hud;
    [SerializeField]
    GameObject _winScreen;

    [SerializeField]
    TextMeshProUGUI _winCrossingsCounter;
    [SerializeField]
    TextMeshProUGUI _winOptimalCrossings;

    [Space]
    [SerializeField]
    GameObject _failScreen;
    [SerializeField]
    GameObject _loadingScreen;




    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        //_levelInfo.GetComponent<RectTransform>().sizeDelta = new Vector2(1, _levelInfo.GetComponent<RectTransform>().sizeDelta.y);
        yield return null;
    }

    public void SetState(UIState state)
    {
        _state = state;
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

                string totalCrossingsString = _winCrossingsCounter.text;
                totalCrossingsString = totalCrossingsString.Replace(NUMBER_VALUE, _crossingsCounterText.text);
                _winCrossingsCounter.text = totalCrossingsString;

                
                break;
            case UIState.Fail:
                _hud.SetActive(false);
                _winScreen.SetActive(false);
                _failScreen.SetActive(true);
                InputSystem.InputEnabled = false;
                break;
        }
    }

    internal void SetLevelDetails(Level level)
    {
        // Set level description text
        _descriptionText.text = $"{level.name}:\n{level}";

        string optimalCrossingsString = _winOptimalCrossings.text;
        optimalCrossingsString = optimalCrossingsString.Replace(NUMBER_VALUE, level.OptimalCrossings.ToString());
        _winOptimalCrossings.text = optimalCrossingsString;

        // Clear previous rules
        foreach (var g in _rules.Values)
        {
            g.transform.SetParent(null);
            Destroy(g);
        }
        _rules.Clear();

        // Load new rules
        _ruleList.SetActive(level.Rules.Length > 0);
        foreach (var rule in level.Rules)
        {
            if (rule != level.Rules[0])
            {
                GameObject line = GameObject.Instantiate(_ruleSeparatorPrefab);
                line.transform.SetParent(_ruleList.transform);
                line.transform.localScale = Vector3.one;

                _rules.Add(new Rule(), line);
            }

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

            icon.SetNativeSize();

            _rules.Add(rule, g);
        }

        // Clear old informations
        foreach (var g in _infos.Values)
        {
            g.transform.SetParent(null);
            Destroy(g);
        }
        _infos.Clear();

        // Show new information if necessary
        _maxWeight = level.BoatMaxWeightAllowed;
        _maxTime = level.BoatMaxTravelCost;

        bool showWeight = _maxWeight > 0;
        _weigthText.gameObject.transform.parent.gameObject.SetActive(showWeight);
        bool showTravelCost = _maxTime > 0;
        _travelTimeText.gameObject.transform.parent.gameObject.SetActive(showTravelCost);

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


    internal void SetGameState(State state)
    {
        _crossingsCounterText.text = state.Crossings.ToString();

        _weigthText.text = $"{state.BoatCurrentWeight} / {_maxWeight}kg";
        _travelTimeText.text = $"{state.BoatTravelCost} / {_maxTime}min";
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


#if UNITY_EDITOR
    private void OnValidate()
    {
        SetState(_state); 
    }

#endif
}
