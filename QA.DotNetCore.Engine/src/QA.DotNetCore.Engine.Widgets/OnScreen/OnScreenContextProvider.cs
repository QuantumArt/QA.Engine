using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace QA.DotNetCore.Engine.Widgets.OnScreen
{
    public class OnScreenContextProvider : IOnScreenContextProvider
    {
        public OnScreenContext GetContext(HttpContext httpContext)
        {
            //return null;
            return new OnScreenContext
            {
                IsAuthorised = true,
                IsEditMode = true,
                User = new OnScreenUser
                {
                    UserId = 1
                }
            };
        }
    }
}
