using System.Collections.Concurrent;
using bstate.core.Middlewares;

namespace bstate.core.Services;

public interface IMiddlewareRegister
{
    public void AddPreprocessor<T>() where T : class, IPreProcessor;
    public void AddPostprocessor<T>() where T : class, IPostProcessor;
    IEnumerable<Type> GetPreprocessors();
    IEnumerable<Type> GetPostprocessors();
}


class MiddlewareRegister : IMiddlewareRegister
{
    private readonly ConcurrentBag<Type> _preprocessors = new();
    private readonly ConcurrentBag<Type> _postprocessors = new();

    public void AddPreprocessor<T>() where T : class, IPreProcessor
    {
        _preprocessors.Add(typeof(T));
    }

    public void AddPostprocessor<T>() where T : class, IPostProcessor
    {
        _postprocessors.Add(typeof(T));
    }

    public IEnumerable<Type> GetPreprocessors() => _preprocessors;

    public IEnumerable<Type> GetPostprocessors() => _postprocessors;
}