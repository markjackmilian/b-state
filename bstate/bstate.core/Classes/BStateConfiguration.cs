using bstate.core.Middlewares;
using bstate.core.Services;

namespace bstate.core.Classes;

public class BStateConfiguration
{
    public IMiddlewareRegister MiddlewareRegister { get; } = new MiddlewareRegister();
    
    public BStateConfiguration AddPreprocessor<T>() where T : class, IPreprocessorMiddleware
    {
        MiddlewareRegister.AddPreprocessor<T>();
        return this;
    }

    public BStateConfiguration AddPostprocessor<T>() where T : class, IPostProcessorMiddleware 
    {
        MiddlewareRegister.AddPostprocessor<T>();
        return this;
    }
}