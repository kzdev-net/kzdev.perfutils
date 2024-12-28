# StringBuilderCache

Since strings in .NET are immutable, building out strings from multiple parts can be memory expensive. Strings are one of the most memory-costly types in the entire system because they hold many characters, so building strings with many intermediate steps can be very expensive because an entirely new string instance is allocated for each step. The [`Concat`](xref:System.String.Concat*) method on the [`String`](xref:System.String) type is helpful in cases where a small number of string segments that are immediately available are being concatenated. Still, it is inefficient to build strings from many parts with logical processing between each step.

The [`StringBuilder`](xref:System.Text.StringBuilder) class is a mutable string class that can be used to build strings more memory efficiently. However, the StringBuilder class has its own memory issues, as it can still allocate a lot of memory for its internal buffer and for the `StringBuilder` instances themselves.

The .NET core library has an internal helper class called `StringBuilderCache` that relieves some of this memory pressure. Still, there are many use cases in every line of business application where caching StringBuilder instances would help application performance, and that internal caching class is not available publicly.

The [`StringBuilderCache`](xref:KZDev.PerfUtils.StringBuilderCache) class in this library is a similar static class that provides a pool of `StringBuilder` instances that can be reused. This can be very helpful in scenarios where many StringBuilder instances are created and disposed of frequently, such as in a loop or an often called method. This caching can have a significant positive impact on applications for overall performance as well as less memory pressure.

## Usage

Using `StringBuilderCache` is very simple. The class provides a static method, `Acquire`, that returns a `StringBuilder` instance from the pool. When you are done with the StringBuilder instance, you return the instance to the pool either directly with a call to the [`Release`](xref:KZDev.PerfUtils.StringBuilderCache.Release) method on the `StringBuilderCache` class, or you can get the built string and return the `StringBuilder` instance to the pool in one step with the [`GetStringAndRelease`](xref:KZDev.PerfUtils.StringBuilderCache.GetStringAndRelease) method.

```csharp
using KZDev.PerfUtils;

class Program
{
	static void Main()
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		stringBuilder.Append("Hello, ");
		stringBuilder.Append("World!");
		Console.WriteLine(stringBuilder.ToString());
		StringBuilderCache.Release(stringBuilder);
	}
}
```

or

```csharp
using KZDev.PerfUtils;

class Program
{
	static void Main()
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		stringBuilder.Append("Hello, ");
		stringBuilder.Append("World!");
		Console.WriteLine(StringBuilderCache.GetStringAndRelease(stringBuilder));
	}
}
```

To ensure that the `StringBuilder` instance is returned to the pool, you must call the `Release` method on the `StringBuilderCache` class or use the `GetStringAndRelease` method. If the `StringBuilder` is not returned to the cache, it will simply get garbage collected like any other object, but you will not get the full performance benefit of having the cache in those cases. 

For cases where an exception may be thrown and to be sure the `StringBuilder` is still returned to the cache, you should use a try-finally block to release the instance.

```csharp
using KZDev.PerfUtils;

class Program
{
	static void Main()
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		try
		{
			stringBuilder.Append("Hello, ");
			stringBuilder.Append("World!");
			Console.WriteLine(stringBuilder.ToString());
		}
		finally
		{
			StringBuilderCache.Release(stringBuilder);
		}
	}
}
```

Alternatively, you can use the `StringBuilderCache` class with the `using` statement to get an instance of [`StringBuilderScope`](xref:KZDev.PerfUtils.StringBuilderScope) to ensure that the `StringBuilder` instance is returned to the pool when you are done with it.

```csharp
using KZDev.PerfUtils;

class Program
{
	static void Main()
	{
		using StringBuilderScope builderScope = StringBuilderCache.GetScope();
		StringBuilder builder = builderScope.Builder;
		builder.Append("Hello, ");
		builder.Append("World!");
		Console.WriteLine(builderScope.ToString());
	}
}
```

The `StringBuilderCache` class is thread-safe so that you can use it in multi-threaded scenarios without issue.