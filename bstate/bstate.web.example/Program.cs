using System.Reflection;
using bstate.core;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using bstate.web.example;
using bstate.web.example.Pipeline;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBState(configuration =>
{
    configuration.RegisterFrom(Assembly.GetExecutingAssembly());

    configuration.AddBehaviour<LogBehaviour>();
    
    
    // configuration.AddOpenRequestPreProcessor(typeof(TestPreprocessorGeneric<>));
    // configuration.AddOpenRequestPreProcessor(typeof(TestPreprocessorOnlyLongGeneric<>));
    // configuration.AddOpenRequestPostProcessor(typeof(TestPostprocessorMiddleware<>));
    
    // configuration.Services.AddTransient<WeatherInitialize>();
});




await builder.Build().RunAsync();