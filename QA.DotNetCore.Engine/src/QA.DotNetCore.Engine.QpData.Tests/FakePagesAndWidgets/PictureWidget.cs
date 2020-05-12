using QA.DotNetCore.Engine.QpData.Replacements;
using System;
using System.Collections.Generic;
using System.Text;

namespace QA.DotNetCore.Engine.QpData.Tests.FakePagesAndWidgets
{
    public class PictureWidget : AbstractWidget
    {
        public string Icon => GetDetail("Icon", string.Empty);

        public string ImageUrl => GetDetail("Image", string.Empty);

        public string Description => GetDetail("Description", string.Empty);
    }
}
