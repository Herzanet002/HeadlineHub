using System.Text.RegularExpressions;
using HeadlineHub.Domain.Common;

namespace HeadlineHub.Application.Common.Extensions;

public static partial class HelperExtensions
{
    public static string StripHtml(this string input)
    {
        return HtmlRegex().Replace(input, string.Empty);
    }

    [GeneratedRegex("<.*?>")]
    private static partial Regex HtmlRegex();

    public static ContentModel CreateContentModel(this CustomSyndicationItem element)
        => new()
        {
            Title = element.OriginalItem.Title.Text,
            Description = element.OriginalItem.Summary?.Text.StripHtml()
                .Replace(Environment.NewLine, string.Empty).Trim(),
            PubDate = element.OriginalItem.PublishDate.LocalDateTime,
            Link = element.OriginalItem.Links[0].Uri.ToString(),
            ImageUrl = element.OriginalItem.Links.Count == 2
                ? element.OriginalItem.Links[1].Uri.ToString()
                : null,
            FeederId = element.FeederId,
            Categories = element.OriginalItem.Categories.Select(x => x.Name).ToList()
        };
}