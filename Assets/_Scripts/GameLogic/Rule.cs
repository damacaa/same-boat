
[System.Serializable]
public class Rule
{
    public TransportableSO A;
    public RuleType comparison;
    public TransportableSO B;

    public enum RuleType
    {
        CantCoexist = 0,
        CountMustBeGreaterThan = 1,
        CountMustBeGreaterEqualThan = 2,
        Requires = 3,
    }

    public enum Result
    {
        AWillEatB,
        AWillFightWithB,
    }

    public bool Evaluate(Transportable[] transportables)
    {
        return true;
    }

    public string ToString(Localization.Language language)
    {
        switch (language)
        {
            default:
            case Localization.Language.En:
                return GetTextEn();
            case Localization.Language.Es:
                return GetTextEs();

        }
    }

    private string GetTextEn()
    {
        switch (comparison)
        {
            case RuleType.CantCoexist:
                return $"A {A.name.ToLower()} and a {B.name.ToLower()} must never be left alone on the same island.";
            case RuleType.CountMustBeGreaterThan:
                return $"The number of {A.NamePlural.ToLower()} on any island must always be greater than the number of {B.NamePlural.ToLower()}.";
            case RuleType.CountMustBeGreaterEqualThan:
                return $"The number of {A.NamePlural.ToLower()} on any island must always be greater than or equal to the number of {B.NamePlural.ToLower()}.";
            case RuleType.Requires:
                return $"A {A.name.ToLower()} must always be accompanied by a {B.name.ToLower()}.";
            default:
                return "NOT IMPLEMENTED";
        }
    }

    private string GetTextEs()
    {
        switch (comparison)
        {
            case RuleType.CantCoexist:
                return $"No puedes dejar un {A.name.ToLower()} y un {B.name.ToLower()} solos en la misma isla.";
            case RuleType.CountMustBeGreaterThan:
                return $"El número de {A.NamePlural.ToLower()} en cada isla siempre debe ser mayor al número de {B.NamePlural.ToLower()}.";
            case RuleType.CountMustBeGreaterEqualThan:
                return $"El número de {A.NamePlural.ToLower()} en cada isla siempre debe ser mayor o igual al número de {B.NamePlural.ToLower()}.";
            case RuleType.Requires:
                return $"Un {A.name.ToLower()} siempre debe estar acompañado por un {B.name.ToLower()} si se queda sólo en una isla.";
            default:
                return "NOT IMPLEMENTED";
        }
    }
}
