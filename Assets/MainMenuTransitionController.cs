using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTransitionController : MonoBehaviour
{
    Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OpenLevelSelection()
    {
        _animator.SetTrigger("OpenLevelSelection");
    }

    public void CloseLevelSelection()
    {
        _animator.SetTrigger("CloseLevelSelection");
    }
}
