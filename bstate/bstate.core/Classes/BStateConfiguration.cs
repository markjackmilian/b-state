using System.Reflection;
using bstate.core.Middlewares;
using bstate.core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace bstate.core.Classes;

public class BStateConfiguration(IServiceCollection services)
{
    public IBehaviourRegister BehaviourRegister { get; } = new BehaviourRegister();
    public List<Assembly> LoadAssemblies { get; } = [];
    
    public IServiceCollection Services => services;

    public BStateConfiguration RegisterFrom(params Assembly[] assemblies)
    {
        LoadAssemblies.AddRange(assemblies);
        return this;
    }

    public BStateConfiguration RegisterFromAssemblyOfType<T>()
    {
        LoadAssemblies.Add(typeof(T).Assembly);
        return this;   
    }
    
    public BStateConfiguration AddOpenRequestPreProcessor(Type openBehaviorType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        if (!openBehaviorType.IsGenericType)
        {
            throw new InvalidOperationException($"{openBehaviorType.Name} must be generic");
        }

        var implementedGenericInterfaces = openBehaviorType.GetInterfaces().Where(i => i.IsGenericType).Select(i => i.GetGenericTypeDefinition());
        var implementedOpenBehaviorInterfaces = new HashSet<Type>(implementedGenericInterfaces.Where(i => i == typeof(IPreProcessorGeneric<>)));

        if (implementedOpenBehaviorInterfaces.Count == 0)
        {
            throw new InvalidOperationException($"{openBehaviorType.Name} must implement {typeof(IPreProcessorGeneric<>).FullName}");
        }

        foreach (var openBehaviorInterface in implementedOpenBehaviorInterfaces)
        {
            RequestPreProcessorsToRegister.Add(new ServiceDescriptor(openBehaviorInterface, openBehaviorType, serviceLifetime));
        }

        return this;
    }
    public List<ServiceDescriptor> RequestPreProcessorsToRegister { get; set; } = new();
    public BStateConfiguration AddOpenRequestPostProcessor(Type openBehaviorType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        if (!openBehaviorType.IsGenericType)
        {
            throw new InvalidOperationException($"{openBehaviorType.Name} must be generic");
        }

        var implementedGenericInterfaces = openBehaviorType.GetInterfaces().Where(i => i.IsGenericType).Select(i => i.GetGenericTypeDefinition());
        var implementedOpenBehaviorInterfaces = new HashSet<Type>(implementedGenericInterfaces.Where(i => i == typeof(IPostProcessorGeneric<>)));

        if (implementedOpenBehaviorInterfaces.Count == 0)
        {
            throw new InvalidOperationException($"{openBehaviorType.Name} must implement {typeof(IPostProcessorGeneric<>).FullName}");
        }

        foreach (var openBehaviorInterface in implementedOpenBehaviorInterfaces)
        {
            RequestPostProcessorsToRegister.Add(new ServiceDescriptor(openBehaviorInterface, openBehaviorType, serviceLifetime));
        }

        return this;
    }

    public List<ServiceDescriptor> RequestPostProcessorsToRegister { get; set; } = new();
    
    public BStateConfiguration AddBehaviour<T>() where T : class, IBehaviour 
    {
        BehaviourRegister.AddBehaviour<T>();
        return this;
    }
    
    
}