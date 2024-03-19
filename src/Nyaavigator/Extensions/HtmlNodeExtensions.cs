using System;
using HtmlAgilityPack;

namespace Nyaavigator.Extensions;

public static class HtmlNodeExtensions
{
    public static bool AttributeContainsInsensitive(this HtmlNode node, string attribute, string text)
    {
        string? classes = node.GetAttributeValue(attribute, null);
        return classes != null && classes.Contains(text, StringComparison.OrdinalIgnoreCase);
    }
}