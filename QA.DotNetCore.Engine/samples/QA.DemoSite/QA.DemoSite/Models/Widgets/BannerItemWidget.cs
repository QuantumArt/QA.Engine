using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.QpData.Replacements;

namespace QA.DemoSite.Models.Widgets
{
    public class BannerItemWidget : AbstractWidget
    {
        public string Header => GetDetail("Header", string.Empty);
        public string Description => GetDetail("Description", string.Empty);
        public string DetailsButtonText => GetDetail("DetailsButtonText", string.Empty);
        public string DetailsButtonUrl => GetDetail("DetailsButtonUrl", string.Empty);

        [LibraryUrl]
        public string BackgroundImage => GetDetail("BackgroundImage", string.Empty);
        [LibraryUrl]
        public string Image => GetDetail("Image", string.Empty);

        public bool ShowButton => !string.IsNullOrWhiteSpace(DetailsButtonText) && !string.IsNullOrWhiteSpace(DetailsButtonUrl);
    }
}
