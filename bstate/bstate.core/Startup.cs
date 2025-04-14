using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;
using bstate.core.Services;

namespace bstate.core;

public static class Startup
{
    public static void AddBState(this IServiceCollection serviceCollection)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        RegisterStates(serviceCollection, assemblies);
        serviceCollection.AddSingleton<IBStateRegister, BStateRegister>();
        serviceCollection.AddSingleton<IStore, Store>();
        serviceCollection.AddSingleton<IBstateRunner, BstateRunner>();
        
        serviceCollection.AddTransient<ActionRunnerNode>();
        serviceCollection.AddTransient<PostProcessorRenderer>();
        
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