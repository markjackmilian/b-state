using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using bstate.core.Classes;
using bstate.core.Middlewares;
using bstate.core.Services;

namespace bstate.core;

public static class Startup
{
    public static void AddBState(this IServiceCollection serviceCollection, Action<BStateConfiguration> builder)
    {
        var configuration = new BStateConfiguration();
        builder(configuration);
        serviceCollection.AddSingleton(configuration);
        foreach (var preProcessor in configuration.MiddlewareRegister.GetPreprocessors())
        {
            serviceCollection.AddTransient(preProcessor);
        }
        foreach (var preProcessor in configuration.MiddlewareRegister.GetPostprocessors())
        {
            serviceCollection.AddTransient(preProcessor);
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        serviceCollection.AddSingleton<IComponentRegister, ComponentRegister>();
        serviceCollection.AddSingleton<IStore, Store>();
        serviceCollection.AddSingleton<IActionChannel, ActionChannel>();
        
        serviceCollection.AddTransient<ActionRunnerMiddleware>();
        serviceCollection.AddTransient<PostProcessorRenderer>();
        
        RegisterStates(serviceCollection, assemblies);
        RegisterActionHandlers(serviceCollection, assemblies);
    }

    private static void RegisterActionHandlers(IServiceCollection serviceCollection, Assembly[] assemblies)
    {
        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false } 
                       && t.GetInterfaces()
                           .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IActionHandler<>)));
    
        foreach (var handlerType in handlerTypes)
        {
            var handlerInterface = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IActionHandler<>));
            serviceCollection.AddTransient(handlerInterface, handlerType);
        }
    }

    private static void RegisterStates(IServiceCollection serviceCollection, Assembly[] assemblies)
    {
        var bstateTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(BState)));

        foreach (var type in bstateTypes)
        {
            serviceCollection.AddSingleton(type);
        }
    }
}