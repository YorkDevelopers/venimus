using Markdig;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace YorkDeveloperEvents.TagHelpers
{
    [HtmlTargetElement("p", Attributes = "markdown")]
    [HtmlTargetElement("markdown")]
    [OutputElementHint("p")]
    public class MarkdownTagHelper : TagHelper
    {
        public ModelExpression Content { get; set; }

        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (output.TagName == "markdown")
            {
                output.TagName = null;
            }
            output.Attributes.RemoveAll("markdown");

            var markdown = await GetContent(output);

            var html = Markdown.ToHtml(FixUnicodeEncoding(markdown));
            output.Content.SetHtmlContent(html ?? "");
        }

        private async Task<string> GetContent(TagHelperOutput output)
        {
            if (Content == null)
                return (await output.GetChildContentAsync()).GetContent();

            return Content.Model?.ToString();
        }

        private string FixUnicodeEncoding(string original) =>
            original.Replace("&#xD;", "\r")
                    .Replace("&#xA;", "\n")
                    .Replace("&#x9;", "\t");
    }
}
