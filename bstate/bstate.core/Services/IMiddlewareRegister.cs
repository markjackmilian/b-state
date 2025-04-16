using System.Collections.Concurrent;
using bstate.core.Middlewares;

namespace bstate.core.Services;

public interface IMiddlewareRegister
{
    public void AddPreprocessor<T>() where T : class, IPreprocessorMiddleware;
    public void AddPostprocessor<T>() where T : class, IPostProcessorMiddleware;
    IEnumerable<Type> GetPreprocessors();
    IEnumerable<Type> GetPostprocessors();
}


class MiddlewareRegister : IMiddlewareRegister
{
    private readonly ConcurrentBag<Type> _preprocessors = new();
    private readonly ConcurrentBag<Type> _postprocessors = new();

    public void AddPreprocessor<T>() where T : class, IPreprocessorMiddleware
    {
        _preprocessors.Add(typeof(T));
    }

    public void AddPostprocessor<T>() where T : class, IPostProcessorMiddleware
    {
        _postprocessors.Add(typeof(T));
    }

    public IEnumerable<Type> GetPreprocessors() => _preprocessors;

    public IEnumerable<Type> GetPostprocessors() => _postprocessors;
}