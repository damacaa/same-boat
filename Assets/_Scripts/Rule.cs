
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
}
