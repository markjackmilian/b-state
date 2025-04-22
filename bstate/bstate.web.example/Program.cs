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

    configuration.AddBehaviour<ExceptionBehaviour>();
    configuration.AddBehaviour<LogBehaviour>();
    
    configuration.AddOpenRequestPreProcessor(typeof(AllRequestPreprocessor<>));
    configuration.AddOpenRequestPreProcessor(typeof(ALongActionPreProcessor<>));
    // configuration.AddOpenRequestPostProcessor(typeof(TestPostprocessorMiddleware<>));
});


await builder.Build().RunAsync();