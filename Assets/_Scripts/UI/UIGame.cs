using Localization;
using Solver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    TextMeshProUGUI _occupiedSeatsCounterText;
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
    GameObject _bigRulePrefab;
    [SerializeField]
    GameObject _ruleSeparatorPrefab;
    [SerializeField]
    GameObject _ruleList;
    [SerializeField]
    GameObject _failedRuleList;
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

    private int _optimalCrossings = 0;

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

    public void SetState(UIState state, State gameState = null)
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

                string totalCrossingsString = _winCrossingsCounter.GetComponent<LocalizedText>().GetCurrentText();
                totalCrossingsString = totalCrossingsString.Replace(NUMBER_VALUE, _crossingsCounterText.text);
                _winCrossingsCounter.text = totalCrossingsString;

                // Set optimal crossings text
                string optimalCrossingsString = _winOptimalCrossings.GetComponent<LocalizedText>().GetCurrentText();
                optimalCrossingsString = optimalCrossingsString.Replace(NUMBER_VALUE, _optimalCrossings.ToString());
                _winOptimalCrossings.text = optimalCrossingsString;


                break;
            case UIState.Fail:
                _hud.SetActive(false);
                _winScreen.SetActive(false);
                _failScreen.SetActive(true);
                InputSystem.InputEnabled = false;

                if (Application.isPlaying)
                {

                    while (_spawnedFailedRules.Count > 0)
                    {
                        Destroy(_spawnedFailedRules.Dequeue());
                    }

                    foreach (var r in GameLogic.BrokenRules)
                    {
                        SpawnFailRule(r, _bigRulePrefab, _failedRuleList, _spawnedFailedRules);
                    }
                }

                break;
        }
    }

    Queue<GameObject> _spawnedFailedRules = new();



    internal void SetLevelDetails(Level level)
    {
        // Set level description text
        _descriptionText.text = $"<b>{level.Name}:</b>\n{level.Description}";

        _optimalCrossings = level.OptimalCrossings;

        // Clear previous rules
        foreach (var g in _rules.Values)
        {
            if (!g)
                continue;

            g.transform.SetParent(null);
            Destroy(g);
        }
        _rules.Clear();

        // Load new rules
        _ruleList.SetActive(level.Rules.Length > 0);
        foreach (var rule in level.Rules)
        {
            // Add a separator between rules
            if (rule != level.Rules[0])
            {
                GameObject line = GameObject.Instantiate(_ruleSeparatorPrefab);
                line.transform.SetParent(_ruleList.transform);
                line.transform.localScale = Vector3.one;

                _rules.Add(new Rule(), line);
            }

            SpawnRule(rule, _rulePrefab, _ruleList, _rules);
        }

        // Clear old information
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

    private void SpawnRule(Rule rule, GameObject prefab, GameObject list, Dictionary<Rule, GameObject> dict = null)
    {
        // Instantiate rule prefab
        GameObject g = GameObject.Instantiate(prefab);
        g.transform.SetParent(list.transform);
        g.transform.localScale = Vector3.one;
        RuleIcon ruleUI = g.GetComponent<RuleIcon>();

        // Get image components
        var imageA = ruleUI.A.GetComponent<Image>();
        var imageA2 = ruleUI.A2.GetComponent<Image>();
        var imageB = ruleUI.B.GetComponent<Image>();

        // Set sprites
        imageB.sprite = rule.B.icon;
        imageA.sprite = rule.A.icon;
        imageA2.sprite = rule.A.icon;

        // Set rule icon
        var icon = ruleUI.Icon.GetComponent<Image>();

        // Show correct icon based on rule
        switch (rule.comparison)
        {
            // A x B
            case Rule.RuleType.CantCoexist:
                icon.sprite = _ruleSprites[0];
                break;
            // A > B
            case Rule.RuleType.CountMustBeGreaterThan:
                icon.sprite = _ruleSprites[1];
                imageA2.gameObject.SetActive(true);
                break;
            // A >= B
            case Rule.RuleType.CountMustBeGreaterEqualThan:
                icon.sprite = _ruleSprites[2];
                imageA2.gameObject.SetActive(true);
                break;
            // A <3 B
            case Rule.RuleType.Requires:
                icon.sprite = _ruleSprites[3];
                break;
        }

        // Set icon size???
        icon.SetNativeSize();

        // Store gameobject
        dict?.Add(rule, g);
    }

    private void SpawnFailRule(Rule rule, GameObject prefab, GameObject list, Queue<GameObject> dict = null)
    {
        // Instantiate rule prefab
        GameObject g = GameObject.Instantiate(prefab);
        g.transform.SetParent(list.transform);
        g.transform.localScale = Vector3.one;
        RuleIcon ruleUI = g.GetComponent<RuleIcon>();

        if (ruleUI.Text)
            ruleUI.Text.text = rule.GetDescription(LocalizationManager.CurrentLanguage);

        // Get image components
        var imageA = ruleUI.A.GetComponent<Image>();
        var imageA2 = ruleUI.A2.GetComponent<Image>();
        var imageB = ruleUI.B.GetComponent<Image>();

        // Set sprites
        imageB.sprite = rule.B.sprite;
        imageA.sprite = rule.A.sprite;
        imageA2.sprite = rule.A.sprite;

        // Set rule icon
        var icon = ruleUI.Icon.GetComponent<Image>();

        // Show correct icon based on rule
        switch (rule.comparison)
        {
            // A x B
            case Rule.RuleType.CantCoexist:
                icon.sprite = _ruleSprites[0];
                imageA2.gameObject.SetActive(false);
                break;
            // A > B
            case Rule.RuleType.CountMustBeGreaterThan:
                icon.sprite = _ruleSprites[1];
                imageA2.gameObject.SetActive(true);
                break;
            // A >= B
            case Rule.RuleType.CountMustBeGreaterEqualThan:
                icon.sprite = _ruleSprites[2];
                imageA2.gameObject.SetActive(true);
                break;
            // A <3 B
            case Rule.RuleType.Requires:
                icon.sprite = _ruleSprites[3];
                imageA2.gameObject.SetActive(false);
                break;
        }

        // Set icon size???
        icon.SetNativeSize();

        // Store gameobject
        dict?.Enqueue(g);
    }

    internal void SetGameState(State state)
    {
        _occupiedSeatsCounterText.text = state.BoatOccupiedSeats + "/" + state.BoatCapacity;
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
