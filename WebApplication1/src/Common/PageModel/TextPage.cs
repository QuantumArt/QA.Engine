using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.PageModel
{
    public class TextPage : AbstractPage
    {
        public string Text { get { return GetDetail("Text", String.Empty); } }
    }
}
