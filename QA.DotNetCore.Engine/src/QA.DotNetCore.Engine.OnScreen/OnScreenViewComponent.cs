using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Routing;
using QA.DotNetCore.Engine.OnScreen.Configuration;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using QA.DotNetCore.Engine.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Razor;

namespace QA.DotNetCore.Engine.OnScreen
{
    public class OnScreenViewComponent : ViewComponent
    {
        private readonly OnScreenSettings _onScreenSettings;

        public OnScreenViewComponent(OnScreenSettings onScreenSettings)
        {
            _onScreenSettings = onScreenSettings;
        }

        public IViewComponentResult Invoke()
        {
            var ctx = ((IOnScreenContextProvider)ViewContext.HttpContext.RequestServices.GetService(typeof(IOnScreenContextProvider)))?.GetContext();
            if (ctx == null)
                throw new InvalidOperationException("OnScreen context not found.");

            if (ctx.Enabled)
            {
                OnScreenViewModel model = new OnScreenViewModel();
                model.StartPage = ViewContext.GetStartPage();
                model.AI = ViewContext.GetCurrentItem();
                model.OnScreenSettings = _onScreenSettings;
                model.Ctx = ctx;
                return View(model);
            }
            return Content(string.Empty);
        }
    }

    public static class OnScreenViewComponentExtensions
    {
        public static IServiceCollection AddOnScreenViewComponent(this IServiceCollection services)
        {
            var onScreenAssembly = typeof(OnScreenViewComponent).Assembly;
            services.AddMvc().AddApplicationPart(onScreenAssembly);
            services.Configure<RazorViewEngineOptions>(options => { options.FileProviders.Add(new EmbeddedFileProvider(onScreenAssembly)); });
            return services;
        }
    }

    public class OnScreenViewModel
    {
        public IStartPage StartPage { get; internal set; }
        public IAbstractItem AI { get; internal set; }
        public OnScreenSettings OnScreenSettings { get; set; }
        public OnScreenContext Ctx { get; set; }
    }
}
