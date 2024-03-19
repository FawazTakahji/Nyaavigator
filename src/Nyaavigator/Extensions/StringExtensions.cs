using System;
using System.Linq;

namespace Nyaavigator.Extensions;

public static class StringExtensions
{
    public static int GetBeginningInt(this string str)
    {
        if (string.IsNullOrEmpty(str) || !char.IsDigit(str[0]))
            throw new ArgumentException("String is null, empty, or doesn't begin with a digit.");

        string result = string.Empty;
        foreach (char c in str)
        {
            if (char.IsDigit(c))
                result += c;
            else
                break;
        }

        return int.Parse(result);
    }

    public static int TryGetBeginningInt(this string? str)
    {
        if (string.IsNullOrEmpty(str) || !char.IsDigit(str[0]))
            return 0;

        string result = string.Empty;
        foreach (char c in str)
        {
            if (char.IsDigit(c))
                result += c;
            else
                break;
        }

        return int.Parse(result);
    }

    public static int GetEndingInt(this string str)
    {
        if (string.IsNullOrEmpty(str) || !char.IsDigit(str[^1]))
            throw new ArgumentException("String is null, empty, or doesn't end with a digit.");

        string result = String.Empty;
        for (int i = str.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(str[i]))
                result += str[i];
            else
                break;
        }

        return int.Parse(result.Reverse().ToArray());
    }

    public static int TryGetEndingInt(this string? str)
    {
        if (string.IsNullOrEmpty(str) || !char.IsDigit(str[^1]))
            return 0;

        string result = String.Empty;
        for (int i = str.Length - 1; i >= 0; i--)
        {
            if (char.IsDigit(str[i]))
                result += str[i];
            else
                break;
        }

        return int.Parse(result.Reverse().ToArray());
    }

    public static string RemoveEndingString(this string str, string substring)
    {
        if (str.EndsWith(substring))
            str = str.Remove(str.Length - substring.Length);

        return str;
    }
}