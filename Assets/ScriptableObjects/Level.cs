using Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 2)]
public class Level : ScriptableObject
{
    public new string name;

    [SerializeField]
    private string[] Names;

    [SerializeField]
    private string[] Descriptions;

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
    public IslandData[] Islands;
    public Rule[] Rules;

    [Space()]
    public int OptimalCrossings = 100;


    public string Name
    {
        get
        {
            return GetName(LocalizationManager.CurrentLanguage);
        }
    }

    public string GetName(Language language)
    {
        int n = Enum.GetValues(typeof(Language)).Length;
        if (Names == null)
        {
            Names = new string[n];
            Names[0] = name;
        }

        if (Names.Length != n)
        {
            string[] newArray = new string[n];
            for (int i = 0; i < Math.Min(Names.Length, n); i++)
            {
                newArray[i] = Names[i];
            }
            Names = newArray;
        }

        return Names[(int)language];
    }

    public void SetName(string n, Language language)
    {
        Names[(int)language] = n;
    }

    public string Description
    {
        get
        {
            return GetDescription(LocalizationManager.CurrentLanguage);
        }
    }

    public string GetDescription(Language language)
    {
        int n = Enum.GetValues(typeof(Language)).Length;
        if (Descriptions == null)
        {
            Descriptions = new string[n];
        }

        if (Descriptions.Length != n)
        {
            string[] newArray = new string[n];
            for (int i = 0; i < Math.Min(Descriptions.Length, n); i++)
            {
                newArray[i] = Descriptions[i];
            }
            Descriptions = newArray;
        }

        return Descriptions[(int)language];
    }

    public void SetDescription(string d, Language language)
    {
        Descriptions[(int)language] = d;
    }


    public string GenerateDescription(Language language)
    {
        switch (language)
        {
            case Language.En:
                return GenerateDescriptionEn();
            case Language.Es:
                return GenerateDescriptionEs();
        }

        return "";
    }

    public string GenerateDescriptionEn()
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

    public string GenerateDescriptionEs()
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
        string[] numbers = new string[] { "Cero", "Uno", "Dos", "Tres", "Cuatro", "Cinco", "Seis", "Siete", "Ocho", "Nueve", "Diez" };

        for (int i = 0; i < arrayOfAllKeys.Length; i++)
        {
            TransportableSO t = arrayOfAllKeys[i];
            int c = count[t];

            listOfTransportables += (i == 0 ? numbers[c] : numbers[c].ToLower()) + " " + (c > 1 ? t.NamePlural.ToLower() : t.name.ToLower());

            if (i == arrayOfAllKeys.Length - 1)
            {
                // Último elemento, no necesita coma ni "y"
            }
            else if (i == arrayOfAllKeys.Length - 2)
            {
                listOfTransportables += (" y ");
            }
            else
            {
                listOfTransportables += (", ");
            }
        }

        sb.Append($"{listOfTransportables} quieren llegar a la isla del norte, pero primero deben cruzar un río.\n");

        // Barco

        bool hasMaxWeight = BoatMaxWeightAllowed > 0;
        bool hasMaxTravelCost = BoatMaxTravelCost > 0;

        sb.Append($"Tienen un barco con {numbers[BoatCapacity].ToLower()}{(BoatCapacity == 1 ? " asiento" : " asientos")}");

        if (hasMaxWeight || hasMaxTravelCost)
        {
            sb.Append(" pero");

            if (hasMaxWeight)
            {
                sb.Append($" solo puede cargar hasta {BoatMaxWeightAllowed * 10} kilos");
                if (hasMaxTravelCost)
                    sb.Append(" y");
                else
                    sb.Append(".\n");
            }

            if (hasMaxTravelCost)
            {
                sb.Append($" solo puede viajar por {BoatMaxTravelCost} minutos.\n");
            }
        }
        else
        {
            sb.Append(".\n");
        }

        foreach (var t in arrayOfAllKeys)
        {
            if (hasMaxWeight || hasMaxTravelCost)
            {
                sb.Append($"   - Un {t.name.ToLower()}");

                if (hasMaxWeight)
                {
                    sb.Append($" pesa {t.Weight * 10} kilos");

                    if (hasMaxTravelCost)
                        sb.Append(" y");
                    else
                        sb.Append(".\n");
                }

                if (hasMaxTravelCost)
                {
                    sb.Append($" tarda {t.TravelCost} {(t.TravelCost > 1 ? "minutos" : "minuto")} para cruzar el río.\n");
                }
            }
        }

        if (hasMaxTravelCost)
        {
            sb.Append($"Cuando dos o más personajes están viajando juntos," +
                $" el tiempo que tomarán para cruzar el río es igual al tiempo que el más lento de ellos tomaría.\n");
        }

        sb.Append($"El barco no se puede mover si no hay alguien conduciéndolo.");

        if (OnlyHumansCanDrive)
        {
            sb.Append($" Sin embargo, el hombre no permitirá que nadie más que él mismo navegue el barco.\n");
        }
        else
        {
            sb.Append("\n");
        }

        // Reglas

        if (Rules.Length == 0)
        {
            return sb.ToString();
        }

        sb.Append($"Aunque todos quieren llegar al otro lado sanos y salvos," +
            $" algunos actuarán según sus instintos animales si se les deja sin supervisión.\n" +
            $"Tenga en cuenta que:\n");

        foreach (var rule in Rules)
        {
            sb.Append("   - " + rule + "\n");
        }

        if (StrictMode)
        {
            sb.Append("\nTenga cuidado, ya que algunos animales están especialmente hambrientos hoy y no podrán " +
                "resistir sus impulsos, incluso en el barco. " +
                "Podrá reconocerlos porque tienen un ícono especial sobre sus cabezas.\n");
        }

        return sb.ToString();
    }


    [System.Serializable]
    public class IslandData
    {
        public TransportableSO[] transportables;
    }
}


