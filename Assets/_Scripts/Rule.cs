
[System.Serializable]
public class Rule
{
    public TransportableSO A;
    public RuleType comparison;
    public TransportableSO B;
    public Result result;

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
                s = "Can't have a " + A.name.ToLower() + " and a " + B.name.ToLower() + " together in the same island";
                break;
            case RuleType.CountMustBeGreaterThan:
                s = "Number of " + A.name.ToLower() + "s must be greater than the number of " + B.name.ToLower() + "s";
                break;
            case RuleType.CountMustBeGreaterEqualThan:
                s = "Number of " + A.name.ToLower() + "s must be greater than or equal to the number of " + B.name.ToLower() + "s";
                break;
            default:
                break;
        }

        switch (result)
        {
            case Result.AWillEatB:
                s += " or it will be eaten.";
                break;
            case Result.AWillFightWithB:
                s += " or they will fight.";
                break;
            default:
                s += ".\n";
                break;
        }

        return s;
    }
}
