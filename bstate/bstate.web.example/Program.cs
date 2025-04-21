using System.Reflection;
using bstate.core;
using bstate.core.Classes;
using bstate.core.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using bstate.web.example;
using bstate.web.example.Components.Features;
using bstate.web.example.Components.Features.RandomTest;
using bstate.web.example.Components.Features.RandomTest.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBState(configuration =>
{
    configuration.RegisterFrom(Assembly.GetExecutingAssembly());

    configuration.AddBehaviour<LogBeaviour>();
    
    
    configuration.AddOpenRequestPreProcessor(typeof(TestPreprocessorGeneric<>));
    configuration.AddOpenRequestPreProcessor(typeof(TestPreprocessorOnlyLongGeneric<>));
    configuration.AddOpenRequestPostProcessor(typeof(TestPostprocessorMiddleware<>));

    configuration.Services.AddTransient<TestCustomLifeCycle>();
});




await builder.Build().RunAsync();