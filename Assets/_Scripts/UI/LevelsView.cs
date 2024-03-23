using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class LevelsView : MonoBehaviour
    {
        [SerializeField]
        private Button _playButton;
        [Header("Preview")]
        [SerializeField]
        private Image _levelPreview;
        [SerializeField]
        private GameObject _transportablesContainer;
        [SerializeField]
        private GameObject _imagePrefab;
        [Header("Levels")]
        [SerializeField]
        private GameObject _levelsContainer;
        [SerializeField]
        private GameObject _levelButtonPrefab;
        [SerializeField]
        private LevelCollection _levels;

        private List<LevelButton> _levelsButtons = new();

        private void Start()
        {
            #region NULL_CHECKS
            if (_levels == null || _levels.Count == 0)
            {
#if UNITY_EDITOR
                Debug.LogError("No available levels to select. Fill with levels scriptables.");
#endif
                return;
            }

            if (_levelButtonPrefab == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Required button prefab not available.");
#endif
                return;
            }

            if (_levelsContainer == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Required container not available.");
#endif
                return;
            }

            if (_playButton == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Play button reference not available.");
#endif
                return;
            }
            #endregion

            _playButton.onClick.AddListener(LoadLevel);

            for (int i = 0; i < _levels.Count; i++)
            {
                //if (!_levels[i].Unlocked)
                //{
                //    continue;
                //}

                GameObject go = Instantiate(_levelButtonPrefab, _levelsContainer.transform);

                if (go.TryGetComponent(out LevelButton levelButton))
                {
                    levelButton.Text = _levels[i].name;
                    levelButton.Init(_levels[i]);
                    levelButton.OnLevelSelected += SelectLevel;

                    _levelsButtons.Add(levelButton);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("Required level button script not found.");
                    return;
#endif
#pragma warning disable CS0162 // Unreachable code detected
                    continue;
#pragma warning restore CS0162 // Unreachable code detected
                }
            }

            if (_levels.Count > 0)
            {
                _levelsButtons[0].Select();
            }
        }

        private void LoadLevel()
        {
            SceneManager.LoadScene(1);
        }

        private void SelectLevel(Level level)
        {

            ProgressManager.Instance.LevelToLoad = level;

            // Set image
            _levelPreview.sprite = level.Preview;
            _levelPreview.preserveAspect = true;

            for (int i = 0; i < _horizontalLayoutGroup.transform.childCount; i++)
            {
                Destroy(_horizontalLayoutGroup.transform.GetChild(i).gameObject);
            }

            foreach (var island in level.Islands)
            {
                foreach (var transportable in island.transportables)
                {
                    var go = GameObject.Instantiate(_characterPrefab);
                    go.transform.SetParent(_horizontalLayoutGroup.transform, false);
                    var image = go.GetComponent<Image>();
                    image.sprite = transportable.sprite;
                }
            }
        }

        [SerializeField]
        HorizontalLayoutGroup _horizontalLayoutGroup;
        [SerializeField]
        private GameObject _characterPrefab;
    }
}