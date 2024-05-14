using Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 2)]
public class Level : ScriptableObject
{
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
            //return GenerateDescription(LocalizationManager.CurrentLanguage);

            string s = GetDescription(LocalizationManager.CurrentLanguage);
            return (!string.IsNullOrEmpty(s)) ? s : GenerateDescription(LocalizationManager.CurrentLanguage);
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


    public void UpdateDescription(Language language)
    {
        SetDescription(GenerateDescription(language), language);
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

        Dictionary<TransportableSO, int> characterCount = new Dictionary<TransportableSO, int>();

        // Count how many of each transportable are there in all islands combined
        foreach (var island in Islands)
        {
            foreach (var t in island.transportables)
            {
                if (t == null)
                    continue;

                TransportableSO key = t;

                if (characterCount.TryGetValue(key, out int value))
                {
                    characterCount[key] = value + 1;
                }
                else
                {
                    characterCount.Add(key, 1);
                }
            }
        }

        ///////////////////////////////////////////////////////////////

        // Detect if there is a man among them
        var man = characterCount.Keys.ToList().Find(x => x.name == "Man");
        bool manIsPresent = man != null;

        if (manIsPresent)
        {
            sb.Append("A man wants to take ");
        }

        // Transforms int to a string
        string[] numbers = new string[] { "Zero", "A", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };

        // Holds a list of transportables
        StringBuilder listOfTransportables = new();

        // Individual keys of all of the types of character that appear
        var transportableGroups = characterCount.Keys.ToArray();

        // Remove man from total count if there is one
        int differentCharacterCount = transportableGroups.Length - (manIsPresent ? 1 : 0);
        int i = 0;
        foreach (TransportableSO transportable in transportableGroups)
        {
            // Don't add the man to the list
            if (transportable.name == "Man")
            {
                continue;
            }

            // How many of this transportable there are
            int transportableAmount = characterCount[transportable];

            bool startWithCapitalLetter = i == 0 && !manIsPresent;

            // Add amount of transportables of this type
            listOfTransportables.Append($"{(startWithCapitalLetter ? numbers[transportableAmount] : numbers[transportableAmount].ToLower())}");
            // Space
            listOfTransportables.Append(" ");
            // Add name, singular or plural depending on amount
            listOfTransportables.Append($"{(transportableAmount > 1 ? transportable.NamePlural.ToLower() : transportable.name.ToLower())}");


            if (i == differentCharacterCount - 1) // Last one
            {
                // DO NOTHING
            }
            else if (i == differentCharacterCount - 2) // Last but one
            {
                listOfTransportables.Append(" and ");
            }
            else // Others
            {
                listOfTransportables.Append(", ");
            }

            i++;
        }


        if (manIsPresent)
            sb.Append($"{listOfTransportables} across the river to reach the island in the north.\n");
        else
            sb.Append($"{listOfTransportables} all want to cross a river to reach the island in the north.\n");

        ///////////////////////////////////////////////////////////////

        // Boat
        bool hasMaxWeight = BoatMaxWeightAllowed > 0;
        bool hasMaxTravelCost = BoatMaxTravelCost > 0;

        if (manIsPresent)
        {
            sb.Append($"However, he only has a small boat with {numbers[BoatCapacity].ToLower()}{(BoatCapacity == 1 ? " seat" : " seats")}");

            if (OnlyHumansCanDrive)
                sb.Append(", which only he is allowed to drive.\n");
            else
                sb.Append(".\n");
        }
        else
        {
            //sb.Append($"They have a boat with {numbers[BoatCapacity].ToLower()}{(BoatCapacity == 1 ? " seat" : " seats")}");
            sb.Append($"However, they only have a small boat with {numbers[BoatCapacity].ToLower()}{(BoatCapacity == 1 ? " seat" : " seats")}, " +
                $"and the boat can't be moved without someone steering it.\n");
        }

        ///////////////////////////////////////////////////////////////

        if (hasMaxWeight || hasMaxTravelCost)
        {

            //sb.Append(" but");
            sb.Append("Also,");

            if (hasMaxWeight)
            {
                sb.Append($" the boat can only carry up to {BoatMaxWeightAllowed * 10} kilos");

                if (hasMaxTravelCost)
                    sb.Append(" and");
                else
                    sb.Append(".\n");
            }

            if (hasMaxTravelCost)
            {
                sb.Append($" the boat can only drive for {BoatMaxTravelCost} minutes.\n");

            }

            // List each transportable weight or travel cost
            foreach (var t in transportableGroups)
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
                    else if (hasMaxTravelCost)
                    {
                        sb.Append($" takes {t.TravelCost} {(t.TravelCost > 1 ? "minutes" : "minute")} to cross the river.\n");
                    }
                }
            }

            if (hasMaxTravelCost)
                sb.Append($"When two or more characters are traveling together," +
                        $" the time they will take to cross the river is equal to the time that the slowest one of them would take.\n");
        }

        ///////////////////////////////////////////////////////////////

        if (Rules.Length > 0)
        {
            sb.Append($"Although everyone wants to reach the island safely, their natural instincts pose a risk when left alone. Keep in mind the following:\n");

            foreach (var rule in Rules)
            {
                sb.Append($"   - {rule.GetDescription(Language.En)}\n");
            }

            if (StrictMode)
            {
                sb.Append("Be carefull, as some animals are specially hugry today and won't be able " +
                    "to resist their urges, even in the boat. " +
                    "You will see them because they have a especial icon over their heads.\n");
            }
        }

        return sb.ToString();
    }

    struct TranslationError
    {
        public string error, solution;
    }

    public string GenerateDescriptionEs()
    {
        StringBuilder sb = new StringBuilder();

        Dictionary<TransportableSO, int> characterCount = new Dictionary<TransportableSO, int>();

        // Contar cuántos de cada transportable hay en todas las islas combinadas
        foreach (var island in Islands)
        {
            foreach (var t in island.transportables)
            {
                if (t == null)
                    continue;

                TransportableSO key = t;

                if (characterCount.TryGetValue(key, out int value))
                {
                    characterCount[key] = value + 1;
                }
                else
                {
                    characterCount.Add(key, 1);
                }
            }
        }

        ///////////////////////////////////////////////////////////////

        // Detectar si hay un hombre entre ellos
        var man = characterCount.Keys.ToList().Find(x => x.name == "Man");
        bool manIsPresent = man != null;

        if (manIsPresent)
        {
            sb.Append("Un hombre quiere llevar ");
        }

        // Transformar int a una cadena
        string[] numbers = new string[] { "Cero", "Un", "Dos", "Tres", "Cuatro", "Cinco", "Seis", "Siete", "Ocho", "Nueve", "Diez" };

        // Mantener una lista de transportables
        StringBuilder listOfTransportables = new();

        // Claves individuales de todos los tipos de personajes que aparecen
        var transportableGroups = characterCount.Keys.ToArray();

        // Eliminar al hombre del conteo total si hay uno
        int differentCharacterCount = transportableGroups.Length - (manIsPresent ? 1 : 0);
        int i = 0;
        foreach (TransportableSO transportable in transportableGroups)
        {
            // No agregar al hombre a la lista
            if (transportable.name == "Man")
            {
                continue;
            }

            // Cuántos de este transportable hay
            int transportableAmount = characterCount[transportable];

            bool startWithCapitalLetter = i == 0 && !manIsPresent;

            // Agregar la cantidad de transportables de este tipo
            listOfTransportables.Append($"{(startWithCapitalLetter ? numbers[transportableAmount] : numbers[transportableAmount].ToLower())}");
            // Espacio
            listOfTransportables.Append(" ");
            // Agregar nombre, en singular o plural dependiendo de la cantidad
            listOfTransportables.Append($"{(transportableAmount > 1 ? transportable.NamePlural.ToLower() : transportable.name.ToLower())}");

            if (i == differentCharacterCount - 1) // Último
            {
                // NO HACER NADA
            }
            else if (i == differentCharacterCount - 2) // Penúltimo
            {
                listOfTransportables.Append(" y ");
            }
            else // Otros
            {
                listOfTransportables.Append(", ");
            }

            i++;
        }

        if (manIsPresent)
            sb.Append($"{listOfTransportables} al otro lado del río para llegar a la isla del norte.\n");
        else
            sb.Append($"{listOfTransportables} quieren cruzar un río para llegar a la isla del norte.\n");

        ///////////////////////////////////////////////////////////////

        // Barco
        bool hasMaxWeight = BoatMaxWeightAllowed > 0;
        bool hasMaxTravelCost = BoatMaxTravelCost > 0;

        if (manIsPresent)
        {
            sb.Append($"Sin embargo, solo tiene un pequeño barco con {numbers[BoatCapacity].ToLower()}{(BoatCapacity == 1 ? " asiento" : " asientos")}");

            if (OnlyHumansCanDrive)
                sb.Append(", que solo él puede conducir.\n");
            else
                sb.Append(".\n");
        }
        else
        {
            sb.Append($"Sin embargo, solo tienen un pequeño barco con {numbers[BoatCapacity].ToLower()}{(BoatCapacity == 1 ? " asiento" : " asientos")}, " +
                $"y el barco no se puede mover sin alguien que lo dirija.\n");
        }

        ///////////////////////////////////////////////////////////////

        if (hasMaxWeight || hasMaxTravelCost)
        {
            sb.Append("Además,");

            if (hasMaxWeight)
            {
                sb.Append($" el barco solo puede llevar hasta {BoatMaxWeightAllowed * 10} kilos");

                if (hasMaxTravelCost)
                    sb.Append(" y");
                else
                    sb.Append(".\n");
            }

            if (hasMaxTravelCost)
            {
                sb.Append($" el barco solo puede navegar por {BoatMaxTravelCost} minutos.\n");
            }

            // Listar el peso o costo de viaje de cada transportable
            foreach (var t in transportableGroups)
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
                    else if (hasMaxTravelCost)
                    {
                        sb.Append($" toma {t.TravelCost} {(t.TravelCost > 1 ? "minutos" : "minuto")} para cruzar el río.\n");
                    }
                }
            }

            if (hasMaxTravelCost)
                sb.Append($"Cuando dos o más personajes viajan juntos," +
                        $" el tiempo que tomarán para cruzar el río es igual al tiempo que el más lento de ellos tomaría.\n");
        }

        ///////////////////////////////////////////////////////////////

        if (Rules.Length > 0)
        {
            sb.Append($"Aunque todos quieren llegar a la isla sanos y salvos, sus instintos representan un riesgo cuando se quedan solos. Ten en cuenta lo siguiente:\n");

            foreach (var rule in Rules)
            {
                sb.Append($"   - {rule.GetDescription(Language.Es)}\n");
            }

            if (StrictMode)
            {
                sb.Append("Ten cuidado, ya que algunos animales están especialmente hambrientos hoy y no podrán " +
                    "resistir sus impulsos, incluso en el barco. " +
                    "Los verá porque tienen un icono especial sobre sus cabezas.\n");
            }
        }


    

        List<TranslationError> commonErrorss = new() {
            new TranslationError{error = "foxes", solution = "zorros"},
            new TranslationError{error = "fox", solution = "zorro"},
            new TranslationError{error = "chickens", solution = "gallinas"},
            new TranslationError{error = "un chicken", solution = "una gallina"},
            new TranslationError{error = "un gallina", solution = "una gallina"},
            new TranslationError{error = "sack of corn", solution = "saco de maíz"},
            new TranslationError{error = "un sheep", solution = "una oveja"},
            new TranslationError{error = "sheep", solution = "ovejas"},
            new TranslationError{error = "wolf", solution = "lobo"},
            new TranslationError{error = "wolves", solution = "lobos"},
            new TranslationError{error = "mouse", solution = "ratón"},
            new TranslationError{error = "mice", solution = "ratones"},
            new TranslationError{error = "cat", solution = "gato"},
            new TranslationError{error = "cats", solution = "gatos"},
            new TranslationError{error = "#", solution = ""},
        };




        // Create a TextInfo object for the current culture
        //TextInfo textInfo = new CultureInfo("en-US").TextInfo;
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

        string result = sb.ToString();

        foreach (var ce in commonErrorss)
        {
            string error = ce.error;
            string solution = ce.solution;
            result = result.Replace(error, solution);

            string errorCapital = ToCapital(error);
            string solutionCapital = ToCapital(solution);
            result = result.Replace(errorCapital, solutionCapital);
        }

        return result;
    }

    private string ToCapital(string text)
    {
        if (text == null || text.Length == 0)
            return text;

        char[] chars = text.ToCharArray();
        chars[0] = char.ToUpper(chars[0]);
        return new string(chars);
    }


    [System.Serializable]
    public class IslandData
    {
        public TransportableSO[] transportables;
    }
}


