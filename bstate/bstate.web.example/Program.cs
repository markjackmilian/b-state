using System.Reflection;
using bstate.core;
using bstate.core.Classes;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using bstate.web.example;
using bstate.web.example.Components.Features.RandomTest;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBState(configuration =>
{
    configuration.RegisterFrom(Assembly.GetExecutingAssembly());

    configuration.AddBeaviour<LogBeaviour>();
    configuration.AddPreprocessor<TestPreprocessorMiddleware>();
    configuration.AddPostprocessor<TestPostprocessorMiddleware>();
});

await builder.Build().RunAsync();