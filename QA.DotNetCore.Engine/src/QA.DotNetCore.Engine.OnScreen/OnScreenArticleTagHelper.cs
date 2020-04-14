using Microsoft.AspNetCore.Razor.TagHelpers;
using QA.DotNetCore.Engine.Abstractions.OnScreen;

namespace QA.DotNetCore.Engine.OnScreen
{
    [HtmlTargetElement("onscreen-article")]
    public class OnScreenArticleTagHelper : TagHelper
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ContentId { get; set; }
        public bool Published { get; set; }

        private readonly IOnScreenContextProvider _onScreenContextProvider;

        public OnScreenArticleTagHelper(IOnScreenContextProvider onScreenContextProvider)
        {
            _onScreenContextProvider = onScreenContextProvider;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var onScreenContext = _onScreenContextProvider.GetContext();
            var isEditMode = onScreenContext != null ? onScreenContext.HasFeature(OnScreenFeatures.Widgets) : false;
            if (isEditMode)
            {
                output.PreContent.SetHtmlContent($"<!--start article {Id} {{ title='{Title}' contentId='{ContentId}' published='{(Published ? "true" : "false")}' }} -->");
                output.PostContent.SetHtmlContent($"<!--end article {Id}-->");
            }
            else
            {
                output.PreContent.SetHtmlContent($"<!--start article-->");
                output.PostContent.SetHtmlContent($"<!--end article-->");
            }
            output.TagName = null;
            base.Process(context, output);
        }
    }
}
