using System.Reflection;
using bstate.core.Middlewares;
using bstate.core.Services;

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
        MiddlewareRegister.AddPreprocessor<T>();
        return this;
    }

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