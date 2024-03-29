
[System.Serializable]
public class Rule
{
    public TransportableSO A;
    public RuleType comparison;
    public TransportableSO B;

    public enum RuleType
    {
        WantsToEat = 0,
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
            case RuleType.WantsToEat:
                s = $"You can't leave a {A.name.ToLower()} and a {B.name.ToLower()} together in the same island or it will be eaten.";
                break;
            case RuleType.CountMustBeGreaterThan:
                s = $"The number of {A.NamePlural.ToLower()} must be greater than the number of {B.NamePlural.ToLower()}.";
                break;
            case RuleType.CountMustBeGreaterEqualThan:
                s = $"The number of {A.NamePlural.ToLower()} must be greater than or equal to the number of {B.NamePlural.ToLower()}.";
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
