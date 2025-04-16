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
        RegisterStates(serviceCollection, assemblies);
        serviceCollection.AddSingleton<IComponentRegister, ComponentRegister>();
        serviceCollection.AddSingleton<IStore, Store>();
        serviceCollection.AddSingleton<IBstateRunner, BstateRunner>();
        //serviceCollection.AddSingleton<IMiddlewareRegister, MiddlewareRegister>();
        
        serviceCollection.AddTransient<ActionRunnerMiddleware>();
        serviceCollection.AddTransient<PostProcessorRenderer>();
        
        RegisterActionHandlers(serviceCollection, assemblies);
    }

    private static void RegisterMiddlewares(IServiceCollection serviceCollection, Assembly[] assemblies)
    {
        var middlewareTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false } 
                       && (typeof(IPreprocessorMiddleware).IsAssignableFrom(t) 
                           || typeof(IPostProcessor).IsAssignableFrom(t)));
    
        foreach (var middlewareType in middlewareTypes)
        {
            if (typeof(IPreprocessorMiddleware).IsAssignableFrom(middlewareType))
                serviceCollection.AddTransient(typeof(IPreprocessorMiddleware), middlewareType);
            
            if (typeof(IPostProcessor).IsAssignableFrom(middlewareType))
                serviceCollection.AddTransient(typeof(IPostProcessor), middlewareType);
        }
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