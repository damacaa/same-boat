using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Transportable", menuName = "ScriptableObjects/Transportable", order = 1)]
public class TransportableSO : ScriptableObject
{
    public new string name;
    public int Weight = 1;
    public bool IsAlive = true;
    public DietType Diet = DietType.None;
    public RuntimeAnimatorController AnimatorController;
    public enum DietType
    {
        None,
        Herbivore,
        Omnivore,
        Carnivore
    }
}