using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Transportable", menuName = "ScriptableObjects/Transportable", order = 1)]
public class TransportableSO : ScriptableObject
{
    public new string name;
    public int size = 1;
    public bool isAlive = true;
    public Diet diet = Diet.None;
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;

    public enum Diet
    {
        None,
        Herbivore,
        Omnivore,
        Carnivore
    }
}