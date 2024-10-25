namespace KZDev.PerfUtils.Examples
{
    public class ConditionXorExample
    {
        private int _flags;

        public bool ToggleFlags (Predicate<int> condition, int flagBits)
        {
            (int originalValue, int newValue) = InterlockedOps.ConditionXor(ref _flags, condition, flagBits);
            return originalValue != newValue;
        }
    }
}
