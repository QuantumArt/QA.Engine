using QA.DotNetCore.Engine.CacheTags.Configuration;
using QA.WidgetPlatform.Api.Application.Middleware;
using QA.WidgetPlatform.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureBaseServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<StatusCodeExceptionHandlerMiddleware>();
//мидлвара для инвалидации кештегов
app.UseCacheTagsInvalidation();
app.UseRouting();
app.UseAuthorization();
app.UseSwaggerUI();
app.MapControllers();
app.MapSwagger();

app.Run();
