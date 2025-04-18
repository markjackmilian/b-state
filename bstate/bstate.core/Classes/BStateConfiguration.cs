using System.Reflection;
using bstate.core.Middlewares;
using bstate.core.Services;

namespace bstate.core.Classes;

public class BStateConfiguration
{
    public IMiddlewareRegister MiddlewareRegister { get; } = new MiddlewareRegister();
    
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
    
    public BStateConfiguration AddBeaviour<T>() where T : class, IBehaviour 
    {
        MiddlewareRegister.AddBeaviour<T>();
        return this;
    }
}