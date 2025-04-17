using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using bstate.core.Classes;
using bstate.core.Middlewares;
using bstate.core.Services;

namespace bstate.core;

/// <summary>
/// Provides extension methods for configuring BState services in the dependency injection container.
/// </summary>
public static class Startup
{
    /// <summary>
    /// Adds BState services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add services to.</param>
    /// <param name="builder">The configuration builder for BState.</param>
    public static void AddBState(this IServiceCollection serviceCollection, Action<BStateConfiguration> builder)
    {
        // Configure BState
        var configuration = new BStateConfiguration();
        builder(configuration);
        serviceCollection.AddSingleton(configuration);
        
        // Get assemblies for type scanning
        var assemblies = configuration.LoadAssemblies.ToArray();
        
        // Register core services
        RegisterCoreServices(serviceCollection);
        
        // Register middlewares
        RegisterMiddlewares(serviceCollection, configuration);
        
        // Register BState types from assemblies
        RegisterDiscoveredTypes(serviceCollection, assemblies);
    }
    
    /// <summary>
    /// Registers the core BState services.
    /// </summary>
    private static void RegisterCoreServices(IServiceCollection serviceCollection)
    {
        // Register singleton services
        serviceCollection.AddSingleton<IComponentRegister, ComponentRegister>();
        serviceCollection.AddSingleton<IStore, Store>();
        serviceCollection.AddSingleton<IBus, Bus>();
        
        // Register transient services
        serviceCollection.AddTransient<ActionRunnerMiddleware>();
        serviceCollection.AddTransient<PostProcessorRenderer>();
    }
    
    /// <summary>
    /// Registers middleware components from the configuration.
    /// </summary>
    private static void RegisterMiddlewares(IServiceCollection serviceCollection, BStateConfiguration configuration)
    {
        // Register preprocessors
        foreach (var preprocessorType in configuration.MiddlewareRegister.GetPreprocessors())
        {
            serviceCollection.AddTransient(preprocessorType);
        }
        
        // Register postprocessors
        foreach (var postprocessorType in configuration.MiddlewareRegister.GetPostprocessors())
        {
            serviceCollection.AddTransient(postprocessorType);
        }
    }
    
    /// <summary>
    /// Registers types discovered through assembly scanning.
    /// </summary>
    private static void RegisterDiscoveredTypes(IServiceCollection serviceCollection, Assembly[] assemblies)
    {
        RegisterStates(serviceCollection, assemblies);
        RegisterActionHandlers(serviceCollection, assemblies);
    }

    /// <summary>
    /// Finds and registers all IActionHandler implementations from the provided assemblies.
    /// </summary>
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

    /// <summary>
    /// Finds and registers all BState implementations from the provided assemblies.
    /// </summary>
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