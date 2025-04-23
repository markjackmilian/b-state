using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using bstate.core.Classes;
using bstate.core.Middlewares;
using bstate.core.Services;
using bstate.core.Services.Lifecycle;
using PipelineNet.Pipelines;

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
    public static void AddBState(this IServiceCollection serviceCollection, Action<IBStateConfiguration> builder)
    {
        // Configure BState
        var configuration = new BStateConfiguration(serviceCollection);
        builder(configuration);
        serviceCollection.AddSingleton<IBStateConfiguration>(configuration);
        
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
        serviceCollection.AddSingleton<IActionBus, ActionBus>();
        serviceCollection.AddSingleton<IPipelineBuilder, PipelineBuilder>();
        
        // Register transient services
        serviceCollection.AddTransient<ActionRunnerMiddleware>();
        serviceCollection.AddTransient<PreProcessorRunnerMiddleware>();
        serviceCollection.AddTransient<PostProcessorRunnerMiddleware>();
        serviceCollection.AddTransient<PostProcessorRenderer>();
    }
    
    /// <summary>
    /// Registers middleware components from the configuration.
    /// </summary>
    private static void RegisterMiddlewares(IServiceCollection serviceCollection, BStateConfiguration configuration)
    {
        foreach (var preprocessorServiceDescriptor in configuration.RequestPreProcessorsToRegister)
        {
            serviceCollection.Add(preprocessorServiceDescriptor);       
        }
        
        foreach (var preprocessorServiceDescriptor in configuration.RequestPostProcessorsToRegister)
        {
            serviceCollection.Add(preprocessorServiceDescriptor);       
        }
        
        // Register beaviours
        foreach (var postprocessorType in configuration.GetBehaviours())
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
        RegisterLifeCycles(serviceCollection, assemblies);
    }

private static void RegisterLifeCycles(IServiceCollection serviceCollection, Assembly[] assemblies)
{
    // Find all non-abstract classes that implement ILifeCycle
    var lifeCycleTypes = assemblies
        .SelectMany(a => a.GetTypes())
        .Where(t => t is { IsClass: true, IsAbstract: false } 
                    && t.GetInterfaces()
                        .Any(i => i == typeof(ILifeCycle) || 
                             (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ILifeCycle))));
                             
    foreach (var lifeCycleType in lifeCycleTypes)
    {
        serviceCollection.AddTransient(lifeCycleType);
    }
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