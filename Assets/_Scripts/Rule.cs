
[System.Serializable]
public class Rule
{
    public TransportableSO A;
    public RuleType comparison;
    public TransportableSO B;

    public enum RuleType
    {
        CantCoexist,
        CountMustBeGreaterThan,
        CountMustBeGreaterEqualThan
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
                s = "Can't have a " + A.name.ToLower() + " and a " + B.name.ToLower() + " together in the same island or it will be eaten.";
                break;
            case RuleType.CountMustBeGreaterThan:
                s = "Number of " + A.NamePlural.ToLower() + " must be greater than the number of " + B.NamePlural.ToLower() + " or they will be eaten.";
                break;
            case RuleType.CountMustBeGreaterEqualThan:
                s = "Number of " + A.NamePlural.ToLower() + " must be greater than or equal to the number of " + B.NamePlural.ToLower() + " or they will be eaten.";
                break;
            default:
                break;
        }


        return s;
    }
}
