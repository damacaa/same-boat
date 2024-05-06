using Localization;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
        TextMeshProUGUI _levelName;
        [SerializeField]
        private Image _levelPreview;
        [SerializeField]
        private GameObject _transportablesContainer;
        [SerializeField]
        private GameObject _imagePrefab;

        [SerializeField]
        HorizontalLayoutGroup _characterList;
        [SerializeField]
        private GameObject _characterPrefab;


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

                if (i > ProgressManager.Instance.CurrentLevel)
                    break;

                GameObject go = Instantiate(_levelButtonPrefab, _levelsContainer.transform);

                if (go.TryGetComponent(out LevelButton levelButton))
                {
                    levelButton.Init(_levels[i]);
                    levelButton.OnLevelSelected += SelectLevel;

                    var localizedText = levelButton.GetComponentInChildren<LocalizedText>();

                    foreach (Language language in Enum.GetValues(typeof(Language)))
                    {
                        localizedText.SetText(language, _levels[i].GetName(language));
                    }
                    localizedText.ForceUpdate();

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

            _levelName.text = level.Name;

            // Set image
            _levelPreview.sprite = level.Preview;
            _levelPreview.preserveAspect = true;

            for (int i = 0; i < _characterList.transform.childCount; i++)
            {
                Destroy(_characterList.transform.GetChild(i).gameObject);
            }

            foreach (var island in level.Islands)
            {
                foreach (var transportable in island.transportables)
                {
                    var go = GameObject.Instantiate(_characterPrefab);
                    go.transform.SetParent(_characterList.transform, false);
                    var image = go.GetComponentInChildren<Image>();
                    image.sprite = transportable.sprite;
                }
            }
        }


    }
}