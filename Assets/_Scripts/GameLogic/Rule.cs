
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

    public override string ToString()
    {
        string s = "";

        switch (comparison)
        {
            case RuleType.CantCoexist:
                s = $"A {A.name.ToLower()} and a {B.name.ToLower()} must never be left alone in the same island.";
                break;
            case RuleType.CountMustBeGreaterThan:
                s = $"The number of {A.NamePlural.ToLower()} on any island must always be greater than the number of {B.NamePlural.ToLower()}.";
                break;
            case RuleType.CountMustBeGreaterEqualThan:
                s = $"The number of {A.NamePlural.ToLower()} on any island must always be greater than or equal to the number of {B.NamePlural.ToLower()}.";
                break;
            case RuleType.Requires:
                s = $"A {A.name.ToLower()} must always be accompanied by a {B.name.ToLower()}.";
                break;
            default:
                break;
        }

        return s;
    }
}
