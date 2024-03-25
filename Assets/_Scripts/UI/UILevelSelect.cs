using UnityEngine;

public class UILevelSelect : MonoBehaviour
{
    //[SerializeField] private MenuUIManager _uiManager;
    //[Range(5, 100)]
    //public int ListSpacing;

    //private VisualElement _root;
    //private VisualElement _canvas;
    //private VisualElement _listContainer;
    //private ListView _uiButtonsList;
    //private Button _backButton;

    //private List<Button> _buttonsList;

    //private int _createdButtonsCount;

    //private void Awake()
    //{
    //    _root = GetComponent<UIDocument>().rootVisualElement;

    //    _canvas = _root.Q<VisualElement>("Canvas");
    //    _canvas.AddToClassList("hide");
    //    _listContainer = _root.Q<VisualElement>("LevelsListContainer");
    //    _uiButtonsList = _root.Q<ListView>("ButtonsList");

    //    _buttonsList = new List<Button>(8);

    //    for (int i = 0; i < ProgressManager.Instance.Levels.Length; i++)
    //    {
    //        Button button = new Button();
    //        _buttonsList.Add(button);
    //    }

    //    _backButton = _root.Q<Button>("Back");
    //    _backButton.clicked += delegate { SoundController.Instace.PlaySound(SoundController.Sound.UI); };
    //    _backButton.clicked += _uiManager.CloseLevelSelect;
    //    _backButton.clicked += _uiManager.OpenMenu;
    //}

    //private void Start()
    //{
    //    Func<VisualElement> makeItem = CreateButton;

    //    Action<VisualElement, int> bindItem = (e, i) => e = _buttonsList[i];

    //    _uiButtonsList = new ListView(_buttonsList, ListSpacing, makeItem, bindItem);
    //    _uiButtonsList.AddToClassList("levels-list");

    //    _listContainer.Add(_uiButtonsList);
    //}

    //public void StartLevel(Level level)
    //{
    //    SceneManager.LoadScene(1);

    //    SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
    //    {
    //        GameManager game = FindObjectOfType<GameManager>();
    //        if (game)
    //            game.LoadLevel(level);
    //    };
    //}

    //protected VisualElement CreateButton()
    //{
    //    if (_createdButtonsCount >= ProgressManager.Instance.Levels.Length)
    //        _createdButtonsCount = 0;

    //    var button = new Button();

    //    button.text = $"Level {_createdButtonsCount + 1}";
    //    button.AddToClassList("level-list-button");
    //    button.clicked += delegate { SoundController.Instace.PlaySound(SoundController.Sound.UI); };
    //    button.clicked += _uiManager.CloseLevelSelect;
    //    button.clicked += _uiManager.CloseMenu;
    //    button.clicked += () =>
    //    {
    //        SceneManager.LoadScene(1);

    //        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
    //        {
    //            GameManager game = FindObjectOfType<GameManager>();
    //            if (game)
    //                game.LoadLevel(ProgressManager.Instance.Levels[_createdButtonsCount]);
    //        };
    //    };

    //    Debug.Log(_createdButtonsCount);
    //    _createdButtonsCount++;

    //    return button;
    //}
}