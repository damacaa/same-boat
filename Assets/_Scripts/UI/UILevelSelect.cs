using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UILevelSelect : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] Level[] _levels;

    private VisualElement _root;
    private VisualElement _canvas;
    private Button _selectLevel1Button;
    private Button _selectLevel2Button;
    private Button _selectLevel3Button;
    private Button _backButton;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _canvas = _root.Q<VisualElement>("Canvas");


        for (int i = 0; i < _levels.Length; i++)
        {

        }


        _selectLevel1Button = _root.Q<Button>("SelectLevel1Button");
        _selectLevel2Button = _root.Q<Button>("SelectLevel2Button");
        _selectLevel3Button = _root.Q<Button>("SelectLevel3Button");
        _backButton = _root.Q<Button>("Back");

        _selectLevel1Button.clicked += delegate { StarLevel(0); };
        _selectLevel2Button.clicked += delegate { StarLevel(1); };
        _selectLevel3Button.clicked += delegate { StarLevel(2); };
        _backButton.clicked += _uiManager.GoToMenu;
    }

    private void StarLevel(int i)
    {
        SceneManager.LoadScene(1);

        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
        {
            GameManager game = FindObjectOfType<GameManager>();
            if (game)
                game.LoadLevel(_levels[i]);
        };

    }
    private void StarLevel1()
    {
        SceneManager.LoadScene(1);

        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
        {
            GameManager game = FindObjectOfType<GameManager>();
            if (game)
                game.LoadLevel(_levels[0]);
        };
    }
    private void StarLevel2()
    {
        SceneManager.LoadScene(1);

        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
        {
            GameManager game = FindObjectOfType<GameManager>();
            if (game)
                game.LoadLevel(_levels[1]);
        };
    }
    private void StarLevel3()
    {
        SceneManager.LoadScene(1);

        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
        {
            GameManager game = FindObjectOfType<GameManager>();
            if (game)
                game.LoadLevel(_levels[2]);
        };
    }
}