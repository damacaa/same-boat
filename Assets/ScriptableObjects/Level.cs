using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 2)]
public class Level : ScriptableObject
{
    public new string name;
    public string Intro = "";
    [Range(1, 3)]
    public int BoatCapacity = 1;
    [Range(0, 10)]
    public int BoatMaxWeightAllowed = 0;
    [Range(0, 100)]
    public int BoatMaxTravelCost = 0;
    public bool CanMoveEmpyBoat = true;
    public Texture2D Map;
    public Level.Island[] Islands;
    public Rule[] Rules;


    public override string ToString()
    {
        string s = "";

        if (name != "")
            s += $"{name}:\n";

        Dictionary<TransportableSO, int> count = new Dictionary<TransportableSO, int>();

        foreach (var island in Islands)
        {
            foreach (var t in island.transportables)
            {
                if (t == null)
                    continue;

                TransportableSO key = t;

                if (count.TryGetValue(key, out int value))
                {
                    count[key] = value + 1;
                }
                else
                {
                    count.Add(key, 1);
                }
            }
        }


        string listOfTransportables = "";

        var arrayOfAllKeys = count.Keys.ToArray();
        string[] numbers = new string[] { "Zero", "A", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };

        for (int i = 0; i < arrayOfAllKeys.Length; i++)
        {
            TransportableSO t = arrayOfAllKeys[i];
            int c = count[t];

            listOfTransportables += (i == 0 ? numbers[c] : numbers[c].ToLower()) + " " + (c > 1 ? t.NamePlural.ToLower() : t.name.ToLower());

            if (i == arrayOfAllKeys.Length - 1)
            {
                //listOfTransportables += ".\n";
            }
            else if (i == arrayOfAllKeys.Length - 2)
            {
                listOfTransportables += " and ";
            }
            else
            {
                listOfTransportables += ", ";
            }
        }


        s += $"{listOfTransportables} are traveling to an island in the north and must cross a river to get there.\n";

        if (Islands.Length == 3)
            s += "Luckyly, there is an extra island.\n";
        else if (Islands.Length > 3)
            s += $"Luckyly, there are {numbers[Islands.Length - 1]} extra islands.\n";

        // Boat

        bool hasMaxWeight = BoatMaxWeightAllowed > 0;
        bool hasMaxTravelCost = BoatMaxTravelCost > 0;

        s += $"They have a boat with {numbers[BoatCapacity].ToLower()}{(BoatCapacity == 1 ? " seat" : " seats")}";

        if (hasMaxWeight || hasMaxTravelCost)
        {

            s += " but";

            if (hasMaxWeight)
            {
                s += $" it can only carry up to {BoatMaxWeightAllowed * 10} kilos";
                if (hasMaxTravelCost)
                    s += " and";
                else
                    s += ".\n";
            }

            if (hasMaxTravelCost)
                s += $" it can only drive for {BoatMaxTravelCost} minutes.\n";
        }
        else
            s += ".\n";




        /*s += Intro;

        int l = Intro.Length;
        if (l > 0 && (Intro[l - 2] != '\\' || Intro[l - 2] != 'n'))
        {
            s += "\n";
        }*/

        // Rules

        foreach (var rule in Rules)
        {
            s += rule + "\n";
        }

        foreach (var t in arrayOfAllKeys)
        {
            if (hasMaxWeight || hasMaxTravelCost)
            {

                s += $"A {t.name.ToLower()}";

                if (hasMaxWeight)
                {
                    s += $" weighs {t.Weight} {(t.Weight > 1 ? " kilos" : " kilo")}";

                    if (hasMaxTravelCost)
                        s += " and";
                    else
                        s += ".\n";
                }

                if (hasMaxTravelCost)
                {
                    s += $" takes {t.TravelCost} {(t.TravelCost > 1 ? " minutes" : " minute")} to cross the river.\n";
                }
            }
        }


        return s;
    }

    [System.Serializable]
    public class Island
    {
        public TransportableSO[] transportables;
    }
}


