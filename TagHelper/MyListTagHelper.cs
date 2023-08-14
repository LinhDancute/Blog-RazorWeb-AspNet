using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AppRazor
{
    // thẻ sẽ là mylist
    [HtmlTargetElement("mylist")]
    public class MyListTagHelper : TagHelper
    {
        // Thuộc tính sẽ là list-title
        public string ListTitle { get; set; }

        // Thuộc tính sẽ là list-items
        public List<string> ListItems { set; get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";    // ul sẽ thay cho myul
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.SetAttribute("class", "list-group");
            output.PreElement.AppendHtml($"<h2>{ListTitle}</h2>");


            StringBuilder content = new StringBuilder();
            foreach (var item in ListItems)
            {
                content.Append($@"<li class=""list-group-item"">{item}</li>");
            }
            output.Content.SetHtmlContent(content.ToString());
        }

    }
}
