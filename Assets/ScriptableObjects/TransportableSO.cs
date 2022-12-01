using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Transportable", menuName = "ScriptableObjects/Transportable", order = 1)]
public class TransportableSO : ScriptableObject
{
    public new string name;
    public string NamePlural;

    public int Weight = 1;
    public int TravelCost = 1;
    public bool CanDrive = true;

    public Sprite sprite;
    public enum DietType
    {
        None,
        Herbivore,
        Omnivore,
        Carnivore
    }

    public AudioClip[] Sounds;
}