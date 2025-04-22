# Interlocked Operations

The `Interlocked` class provides atomic operations for variables shared between threads. These operations are guaranteed to be atomic, meaning they are thread-safe and can be used in a multithreaded environment without requiring locks.

The [`InterlockedOps`](xref:KZDev.PerfUtils.InterlockedOps) class extends the functionality of the Interlocked class by providing additional static helper methods. These methods offer convenience and readability for operations that would otherwise require repetitive and verbose code. By reducing code duplication, `InterlockedOps` makes the codebase easier to understand and maintain.

## Variable Exclusive OR Operations

The `InterlockedOps` class provides the [`Xor`](xref:KZDev.PerfUtils.InterlockedOps.Xor*) method, which performs an atomic XOR operation on a variable. The result of the operation is stored in the provided variable, and the method returns the variable's original value. This method is particularly useful for toggling a bit in an integer variable between 1 and 0 in a thread-safe manner.

The Xor operation supports all integer types, including **int**, **long**, **uint**, and **ulong**.

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

The [`ClearBits`](xref:KZDev.PerfUtils.InterlockedOps.ClearBits*) method clears (sets to 0) one or more bits in a variable in a thread-safe atomic operation. The method returns a value tuple containing:

- The original value of the variable.
- The new value of the variable after the operation.

## Variable SetBits Operations

The [`SetBits`](xref:KZDev.PerfUtils.InterlockedOps.SetBits*) method is a semantically clearer equivalent of the `Interlocked.Or` method. It sets (to 1) one or more bits in a variable in a thread-safe atomic operation. Like `ClearBits`, it returns a value tuple containing:

- The original value of the variable.
- The new value of the variable after the operation.

## Conditional Update Operations

The `InterlockedOps` class provides conditional bitwise update methods for the following operations:

- [`And`](xref:KZDev.PerfUtils.InterlockedOps.ConditionAnd*)
- [`Or`](xref:KZDev.PerfUtils.InterlockedOps.ConditionOr*)
- [`Xor`](xref:KZDev.PerfUtils.InterlockedOps.ConditionXor*)
- [`ClearBits`](xref:KZDev.PerfUtils.InterlockedOps.ConditionClearBits*)
- [`SetBits`](xref:KZDev.PerfUtils.InterlockedOps.ConditionSetBits*)

These methods allow you to update a variable based on a boolean condition, enabling lock-free algorithms that require atomic updates dependent on dynamic conditions. The operation is only applied if the condition is met, ensuring thread safety.

### How Conditional Methods Work

The conditional methods are similar to their non-conditional counterparts but include a predicate delegate. The predicate is called with the current value of the variable and determines whether the operation should be applied. If a race condition occurs and another thread modifies the variable between the predicate call and the operation, the predicate is called again with the updated value. This process continues until the operation is successfully applied or the predicate returns false.

The conditional methods return a value tuple containing:

- The original value of the variable.
- The new value of the variable after the operation.

If the predicate returns false, both values in the tuple will be the same, reflecting the variable's value at the time the predicate returned false.

### Predicate Overloads

To avoid **closures** when the predicate requires additional arguments, overloads of the conditional methods accept a predicate with an additional condition data argument.

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
