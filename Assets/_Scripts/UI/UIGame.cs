using UnityEngine;
using UnityEngine.UIElements;

public class UIGame : MonoBehaviour
{
    private VisualElement _root;
    private Label _numberOfCrossings;

    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _numberOfCrossings = _root.Q<Label>("NumberOfCrossingsValue");
        GameManager.instance.OnLevelLoaded += SetCrossingsChangeListener;
    }

    public void SetCrossingsChangeListener()
    {
        GameManager.instance.Game.Boat.OnCrossingsChange +=
            (int v) => { _numberOfCrossings.text = v.ToString(); };
    }
}