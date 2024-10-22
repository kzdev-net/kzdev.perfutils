# Interlocked Operations

The `Interlocked` class provides atomic operations for variables that are shared between threads. These operations are guaranteed to be atomic, which means that they are thread-safe and can be used in a multithreaded environment without the need for locks.

The [`InterlockedOps`](xref:KZDev.PerfUtils.InterlockedOps) class provides a set of static methods helper methods that provide additional functionality that is not currently available in the `Interlocked` class.

All of the operations provided by the `InterlockedOps` class are convenience methods that can be implemented using the existing `Interlocked` class, similar to the `Interlocked` [`And`](xref:System.Threading.Interlocked.And*) & [`Or`](xref:System.Threading.Interlocked.Or*) helper methods. However, the `InterlockedOps` class provides a more readable, concise, and consistent way to perform the provided operations than repeatedly copying and pasting code. This can make the code easier to understand and maintain by reducing code duplication.

## Variable Exclusive OR Operations

The `InterlockedOps` class provides the `Xor` method, which performs an atomic XOR operation on a variable and returns the original value of the variable. This method is useful for toggling a boolean variable between `true` and `false` in a thread-safe manner.

The operation can be performed on any integer type, including `int`, `long`, `uint`, and `ulong`.

```csharp
public class Example
{
  private int _flag;

  public bool ToggleFlag()
  {
    int originalValue = InterlockedOps.Xor(ref _flag, 1);
    return originalValue == 0;
  }
}
```

## Conditional Update Operations

The `InterlockedOps` class provides conditional bitwise update operations, such as `And`, `Or`, and `Xor`, that allow you to update a variable based on a boolean condition. These methods are useful for implementing lock-free algorithms that require atomic updates to a variable that depend on a dynamic condition.

The conditional `And`, `Or`, and `Xor` methods take a reference to the variable to update, the value to update the variable with, and a boolean condition that determines whether the update should be performed.

```csharp
public class Example
{
  private int _value;

  public void UpdateValue(int newValue, bool condition)
  {
    if (condition)
    {
      InterlockedOps.And(ref _value, newValue);
    }
    else
    {
      InterlockedOps.Or(ref _value, newValue);
    }
  }
}
```