using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 2)]
public class Level : ScriptableObject
{
    public new string name;
    [Range(2, 4)]
    public int BoatCapacity = 2;
    [Range(0, 20)]
    public int BoatMaxWeightAllowed = 0;
    [Range(0, 100)]
    public int BoatMaxTravelCost = 0;
    public bool OnlyHumansCanDrive = false;
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

        s += $"\n{listOfTransportables} want to get to the island in the north, but they must cross a river first.\n";

        // Boat

        bool hasMaxWeight = BoatMaxWeightAllowed > 0;
        bool hasMaxTravelCost = BoatMaxTravelCost > 0;

        s += $"\nThey have a boat with {numbers[BoatCapacity].ToLower()}{(BoatCapacity == 1 ? " seat" : " seats")}";

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
            {
                s += $" it can only drive for {BoatMaxTravelCost} minutes.\n";

            }
        }
        else
            s += ".\n";

        foreach (var t in arrayOfAllKeys)
        {
            if (hasMaxWeight || hasMaxTravelCost)
            {

                s += $"   - A {t.name.ToLower()}";

                if (hasMaxWeight)
                {
                    s += $" weighs {t.Weight*10} kilos";

                    if (hasMaxTravelCost)
                        s += " and";
                    else
                        s += ".\n";
                }

                if (hasMaxTravelCost)
                {
                    s += $" takes {t.TravelCost} {(t.TravelCost > 1 ? "minutes" : "minute")} to cross the river.\n";
                }
            }
        }

        if (hasMaxTravelCost)
            s += $"When two or more things are traveling together," +
                    $" the time they will take to cross the river is equal to the time that the slowest one of them would take.\n";

        s += $"\nThe boat can’t be moved if there isn’t somebody driving it.";

        if (OnlyHumansCanDrive)
            s += $" However, the man won't allow anyone but himself to drive.\n";
        else
        {
            s += "\n";
        }

        // Rules

        if (Rules.Length == 0)
            return s;

        s += $"\nEven though they all want everyone to get to the other side in one piece," +
            $" the animal instincts of some of them will kick in if they are left unattended." +
            $" Keep in mind that:\n\n";

        foreach (var rule in Rules)
        {
            s += "   - " + rule + "\n";
        }

        return s;
    }

    [System.Serializable]
    public class Island
    {
        public TransportableSO[] transportables;
    }
}


