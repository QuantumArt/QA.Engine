using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Engine.Widgets.Configuration;
using QA.DotNetCore.Engine.Widgets.OnScreen;

namespace DemoWebApplication.Components
{
    public class OnScreenViewComponent : ViewComponent
    {
        private readonly OnScreenSettings _onScreenSettings;

        public OnScreenViewComponent(IOptions<OnScreenSettings> onScreenSettings)
        {
            _onScreenSettings = onScreenSettings.Value;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.Yield();

            var model = new OnScreenViewModel
            {
                IsOnscreenMode = IsOnScreenEditMode(ViewContext.HttpContext),
                AdminSiteBaseUrl = _onScreenSettings.AdminSiteBaseUrl

            };


            return View(model);
        }


        private static bool IsOnScreenEditMode(HttpContext httpContext)
        {
            if (!httpContext.Items.ContainsKey(OnScreenModeKeys.OnScreenContext))
                return false;
            if (!(httpContext.Items[OnScreenModeKeys.OnScreenContext] is OnScreenContext context))
                return false;
            return context.IsAuthorised && context.IsEditMode;
        }
    }

    public class OnScreenViewModel
    {
        public bool IsOnscreenMode { get; set; }
        public string AdminSiteBaseUrl { get; set; }
    }
}
