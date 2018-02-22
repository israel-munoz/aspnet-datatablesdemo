namespace DataTablesDemo
{
    using System;

    public static class StringHelper
    {
        public static bool Contains(this string source, string value, bool ignoreCase)
        {
            return source != null &&
                value != null &&
                source.IndexOf(
                    value,
                    ignoreCase
                        ? StringComparison.OrdinalIgnoreCase
                        : StringComparison.Ordinal) >= 0;
        }
    }
}