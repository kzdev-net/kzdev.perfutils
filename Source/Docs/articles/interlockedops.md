# Interlocked Operations

The `Interlocked` class provides atomic operations for variables that are shared between threads. These operations are guaranteed atomic, meaning they are thread-safe and can be used in a multithreaded environment without locking.

The [`InterlockedOps`](xref:KZDev.PerfUtils.InterlockedOps) class provides a set of static helper methods that provide additional functionality that is not currently available in the `Interlocked` class.

All the operations provided by the `InterlockedOps` class are convenience methods that can be implemented directly in code using the existing `Interlocked` class. However, the `InterlockedOps` class offers a more readable, concise, and consistent way to perform the provided operations than repeatedly copying and pasting code. Reducing code duplication can make the code easier to understand and maintain.

## Variable Exclusive OR Operations

The `InterlockedOps` class provides the [`Xor`](xref:KZDev.PerfUtils.InterlockedOps.Xor*) method, which performs an atomic XOR operation on a variable, stores the operation result in the provided variable, and returns the variable's original value. This method is useful for toggling an integer variable bit value between `1` and `0` in a thread-safe manner.

The operation can be performed on any integer type, including `int`, `long`, `uint`, and `ulong`.

```csharp
public class XorExample
{
    private int _flag;

    public bool ToggleFlag ()
    {
        int originalValue = InterlockedOps.Xor(ref _flag, 1);
        return originalValue == 0;
    }
}
```

## Variable ClearBits Operations

The [`ClearBits`](xref:KZDev.PerfUtils.InterlockedOps.ClearBits*) methods are used to clear (set to 0) one or more bits that are currently set (set to 1) in a thread-safe atomic operation. The methods return a value tuple of the original value and a new value of the variable at the instant the operation was applied.

## Variable SetBits Operations

The [`SetBits`](xref:KZDev.PerfUtils.InterlockedOps.SetBits*) methods are semantically clearer equivalents of the `Interlocked` [`Or`](xref:System.Threading.Interlocked.Or*). The difference is that the SetBits method returns a value tuple of the original value and a new variable value when the operation is applied.

## Conditional Update Operations

The `InterlockedOps` class provides conditional bitwise update operations for [`And`](xref:KZDev.PerfUtils.InterlockedOps.ConditionAnd*), [`Or`](xref:KZDev.PerfUtils.InterlockedOps.ConditionOr*), [`Xor`](xref:KZDev.PerfUtils.InterlockedOps.ConditionXor*), [`ClearBits`](xref:KZDev.PerfUtils.InterlockedOps.ConditionClearBits*), and [`SetBits`](xref:KZDev.PerfUtils.InterlockedOps.ConditionSetBits*), allowing you to update a variable based on a boolean condition. These methods help implement lock-free algorithms that require atomic updates to a variable that depends on a dynamic condition and guarantee that the operation only occurs if the condition is met in a thread-safe manner.

The conditional methods are the same as the non-conditional methods, except these methods also take a predicate delegate that gets called with the current value of the variable. The predicate determines whether the corresponding operation should be applied based on the variable's value at that instant. When there is a race condition, and another thread changes the variable's value between the start of the condition delegate call, and applying the operation to the variable, the predicate delegate will be called again with the new variable value and continue until the newly computed value can be applied or the predicate returns `false`.

The conditional methods all return a value tuple with the original variable value and the new value at the instant the operation was performed. If the condition predicate returns false, both value tuple values will be the same and set to the value of the variable passed to the condition predicate that returned false.

Also, overloads of the condition operations take a predicate-argument to avoid closures when the predicate needs additional values to process the conditional logic.

```csharp
public class ConditionXorExample
{
    private int _flags;

    public bool ToggleFlags (Predicate<int> condition, int flagBits)
    {
        (int originalValue, int newValue) = InterlockedOps.ConditionXor(ref _flags, condition, flagBits);
        return originalValue != newValue;
    }
}
```

```csharp
public class ConditionClearBitsExample
{
    private int _flags;

    public bool ClearFlags<T> (Func<int, T, bool> condition, T conditionArgument, int flagBits)
    {
        (int originalValue, int newValue) = InterlockedOps.ConditionClearBits(ref _flags, condition, conditionArgument, flagBits);
        return originalValue != newValue;
    }
}
```
