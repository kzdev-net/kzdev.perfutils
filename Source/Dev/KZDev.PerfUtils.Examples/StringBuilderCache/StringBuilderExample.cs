using System.Text;

namespace KZDev.PerfUtils.Examples
{
    public class StringBuilderExample
    {
        public static void SimpleRelease ()
        {
            StringBuilder stringBuilder = StringBuilderCache.Acquire();
            stringBuilder.Append("Hello, ");
            stringBuilder.Append("World!");
            Console.WriteLine(stringBuilder.ToString());
            StringBuilderCache.Release(stringBuilder);
        }

        public static void GetStringAndRelease ()
        {
            StringBuilder stringBuilder = StringBuilderCache.Acquire();
            stringBuilder.Append("Hello, ");
            stringBuilder.Append("World!");
            Console.WriteLine(StringBuilderCache.GetStringAndRelease(stringBuilder));
        }
    }
}
