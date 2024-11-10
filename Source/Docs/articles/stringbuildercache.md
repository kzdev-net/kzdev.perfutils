# StringBuilderCache

Since strings in .NET are immutable, building out strings from multiple parts can be memory expensive. Strings are one of the most memory-costly types in the entire system because they hold many characters, so building strings with many intermediate steps can be very expensive because an entirely new string instance is allocated for each step. The [`Concat`](xref:System.String.Concat*) method on the [`String`](xref:System.String) type is helpful in cases where a small number of string segments that are immediately available are being concatenated. Still, it is inefficient to build strings from many parts with logical processing between each part.

The [`StringBuilder`](xref:System.Text.StringBuilder) class is a mutable string class that can be used to build strings more memory efficiently. However, the StringBuilder class has its own memory issues, as it can still allocate a lot of memory for its internal buffer and for the `StringBuilder` instances themselves.

The .NET core library has an internal helper class called `StringBuilderCache` that relieves some of this memory pressure. Still, there are many use cases in every application where caching StringBuilder instances would help application performance, and that caching class is not available to the public.

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

The `StringBuilderCache` class is thread-safe so that you can use it in multi-threaded scenarios without issue. The pool of `StringBuilder` instances is created per thread, so each thread has its pool of `StringBuilder` instances. This means that the `StringBuilder` instances are not shared between threads, and the pool is not a global pool that all threads share. This is done to avoid contention between threads for the pool of `StringBuilder` instances. Even with this per-thread approach, there is no issue with releasing a `StringBuilder` instance on a different thread than it was acquired on, so you can pass `StringBuilder` instances between threads without issue, and `StringBuilder` instances can be used in async methods without issue.
