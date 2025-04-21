using System.Reflection;
using bstate.core.Middlewares;
using bstate.core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace bstate.core.Classes;

public class BStateConfiguration
{
    public IMiddlewareRegister MiddlewareRegister { get; } = new MiddlewareRegister();
    
    public readonly List<Type> OnInitializeList = new();

    public List<Assembly> LoadAssemblies { get; } = [];

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

    
    public BStateConfiguration AddPreprocessor<T>() where T : class, IPreProcessor
    {
        //MiddlewareRegister.AddPreprocessor<T>();
        return this;
    }
    
    public BStateConfiguration AddGenericPreprocessor<T,TAction>() where T : class, IPreProcessorGeneric<TAction> where TAction : IAction
    {
        MiddlewareRegister.AddGenericPreprocessor<T,TAction>();
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

    public BStateConfiguration AddPostprocessor<T>() where T : class, IPostProcessor 
    {
        MiddlewareRegister.AddPostprocessor<T>();
        return this;
    }
    
    public BStateConfiguration AddBehaviour<T>() where T : class, IBehaviour 
    {
        MiddlewareRegister.AddBehaviour<T>();
        return this;
    }
    
    public BStateConfiguration AddUseInitialize<T>() where T : class, IOnInitialize
    {
        this.OnInitializeList.Add(typeof(T));
        return this;
    }
    
}