namespace KZDev.PerfUtils.Examples;

public class ConditionClearBitsExample
{
    private int _flags;

    public bool ClearFlags<T> (Func<int, T, bool> condition, T conditionArgument, int flagBits)
    {
        (int originalValue, int newValue) = InterlockedOps.ConditionClearBits(ref _flags, condition, conditionArgument, flagBits);
        return originalValue != newValue;
    }
}