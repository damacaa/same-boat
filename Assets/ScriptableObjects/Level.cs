using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 2)]
public class Level : ScriptableObject
{
    public new string name;
    public string Description;
    public Sprite Preview;
    [Space]
    [Range(2, 4)]
    public int BoatCapacity = 2;
    [Range(0, 20)]
    public int BoatMaxWeightAllowed = 0;
    [Range(0, 100)]
    public int BoatMaxTravelCost = 0;
    [Space()]
    public bool StrictMode = false;
    public bool OnlyHumansCanDrive = false;
    public bool Unlocked = false;
    [Space()]
    public Texture2D Map;
    public Island[] Islands;
    public Rule[] Rules;

    [Space()]
    public int OptimalCrossings = 100;

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(Description))
            return Description;

        return GenerateDescription();
    }

    private string GenerateDescription()
    {
        StringBuilder sb = new StringBuilder();

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
                //listOfTransportablesb.Append( ".\n";
            }
            else if (i == arrayOfAllKeys.Length - 2)
            {
                listOfTransportables += (" and ");
            }
            else
            {
                listOfTransportables += (", ");
            }
        }

        sb.Append($"{listOfTransportables} want to get to the island in the north, but they must cross a river first.\n");

        // Boat

        bool hasMaxWeight = BoatMaxWeightAllowed > 0;
        bool hasMaxTravelCost = BoatMaxTravelCost > 0;

        sb.Append($"They have a boat with {numbers[BoatCapacity].ToLower()}{(BoatCapacity == 1 ? " seat" : " seats")}");

        if (hasMaxWeight || hasMaxTravelCost)
        {

            sb.Append(" but");

            if (hasMaxWeight)
            {
                sb.Append($" it can only carry up to {BoatMaxWeightAllowed * 10} kilos");
                if (hasMaxTravelCost)
                    sb.Append(" and");
                else
                    sb.Append(".\n");
            }

            if (hasMaxTravelCost)
            {
                sb.Append($" it can only drive for {BoatMaxTravelCost} minutes.\n");

            }
        }
        else
            sb.Append(".\n");

        foreach (var t in arrayOfAllKeys)
        {
            if (hasMaxWeight || hasMaxTravelCost)
            {

                sb.Append($"   - A {t.name.ToLower()}");

                if (hasMaxWeight)
                {
                    sb.Append($" weighs {t.Weight * 10} kilos");

                    if (hasMaxTravelCost)
                        sb.Append(" and");
                    else
                        sb.Append(".\n");
                }

                if (hasMaxTravelCost)
                {
                    sb.Append($" takes {t.TravelCost} {(t.TravelCost > 1 ? "minutes" : "minute")} to cross the river.\n");
                }
            }
        }

        if (hasMaxTravelCost)
            sb.Append($"When two or more characters are traveling together," +
                    $" the time they will take to cross the river is equal to the time that the slowest one of them would take.\n");

        sb.Append($"The boat can’t be moved if there isn’t someone driving it.");

        if (OnlyHumansCanDrive)
            sb.Append($" However, the man won't allow anyone but himself to sail the boat.\n");
        else
        {
            sb.Append("\n");
        }

        // Rules

        if (Rules.Length == 0)
            return sb.ToString();

        sb.Append($"Even though they all want everyone to get to the other side in one piece," +
            $" the animal instincts of some of them will kick in if they are left unattended.\n" +
            $"Keep in mind that:\n");

        foreach (var rule in Rules)
        {
            sb.Append("   - " + rule + "\n");
        }

        if (StrictMode)
        {
            sb.Append("\nBe carefull, as some animals are specially hugry today and won't be able " +
                "to resist their urges, even in the boat. " +
                "You will see them because they have an especial icon over their heads.\n");
        }

        return sb.ToString();
    }

    [System.Serializable]
    public class Island
    {
        public TransportableSO[] transportables;
    }
}


