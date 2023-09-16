using System.Text.RegularExpressions;

namespace InfoLinker.Api;

public static partial class StringHelpers
{
    public static string StripHtml(this string input)
    {
        return HtmlRegex().Replace(input, string.Empty);
    }

    [GeneratedRegex("<.*?>")]
    private static partial Regex HtmlRegex();
}