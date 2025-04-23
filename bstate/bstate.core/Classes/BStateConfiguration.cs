using System.Collections.Concurrent;
using System.Reflection;
using bstate.core.Middlewares;
using bstate.core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace bstate.core.Classes;

public interface IBStateConfiguration
{
    
    List<Assembly> LoadAssemblies { get; }
    IServiceCollection Services { get; }
    List<ServiceDescriptor> RequestPreProcessorsToRegister { get; set; }
    List<ServiceDescriptor> RequestPostProcessorsToRegister { get; set; }
    IBStateConfiguration RegisterFrom(params Assembly[] assemblies);
    IBStateConfiguration RegisterFromAssemblyOfType<T>();
    IBStateConfiguration AddOpenRequestPreProcessor(Type openBehaviorType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient);
    IBStateConfiguration AddOpenRequestPostProcessor(Type openBehaviorType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient);
    IBStateConfiguration AddBehaviour<T>() where T : class, IBehaviour;
    IEnumerable<Type> GetBehaviours();
}

public class BStateConfiguration(IServiceCollection services) : IBStateConfiguration
{
    
    public List<Assembly> LoadAssemblies { get; } = [];
    
    public IServiceCollection Services => services;

    public IBStateConfiguration RegisterFrom(params Assembly[] assemblies)
    {
        LoadAssemblies.AddRange(assemblies);
        return this;
    }

    public IBStateConfiguration RegisterFromAssemblyOfType<T>()
    {
        LoadAssemblies.Add(typeof(T).Assembly);
        return this;   
    }
    
    public IBStateConfiguration AddOpenRequestPreProcessor(Type openBehaviorType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        if (!openBehaviorType.IsGenericType)
        {
            throw new InvalidOperationException($"{openBehaviorType.Name} must be generic");
        }

        var implementedGenericInterfaces = openBehaviorType.GetInterfaces().Where(i => i.IsGenericType).Select(i => i.GetGenericTypeDefinition());
        var implementedOpenBehaviorInterfaces = new HashSet<Type>(implementedGenericInterfaces.Where(i => i == typeof(IPreProcessor<>)));

        if (implementedOpenBehaviorInterfaces.Count == 0)
        {
            throw new InvalidOperationException($"{openBehaviorType.Name} must implement {typeof(IPreProcessor<>).FullName}");
        }

        foreach (var openBehaviorInterface in implementedOpenBehaviorInterfaces)
        {
            RequestPreProcessorsToRegister.Add(new ServiceDescriptor(openBehaviorInterface, openBehaviorType, serviceLifetime));
        }

        return this;
    }
    public List<ServiceDescriptor> RequestPreProcessorsToRegister { get; set; } = [];
    public IBStateConfiguration AddOpenRequestPostProcessor(Type openBehaviorType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        if (!openBehaviorType.IsGenericType)
        {
            throw new InvalidOperationException($"{openBehaviorType.Name} must be generic");
        }

        var implementedGenericInterfaces = openBehaviorType.GetInterfaces().Where(i => i.IsGenericType).Select(i => i.GetGenericTypeDefinition());
        var implementedOpenBehaviorInterfaces = new HashSet<Type>(implementedGenericInterfaces.Where(i => i == typeof(IPostProcessor<>)));

        if (implementedOpenBehaviorInterfaces.Count == 0)
        {
            throw new InvalidOperationException($"{openBehaviorType.Name} must implement {typeof(IPostProcessor<>).FullName}");
        }

        foreach (var openBehaviorInterface in implementedOpenBehaviorInterfaces)
        {
            RequestPostProcessorsToRegister.Add(new ServiceDescriptor(openBehaviorInterface, openBehaviorType, serviceLifetime));
        }

        return this;
    }

    public List<ServiceDescriptor> RequestPostProcessorsToRegister { get; set; } = [];
    
    public IBStateConfiguration AddBehaviour<T>() where T : class, IBehaviour 
    {
        _beaviours.Add(typeof(T));

        return this;
    }
    
    private readonly List<Type> _beaviours = [];
    public IEnumerable<Type> GetBehaviours() => _beaviours;
    
}