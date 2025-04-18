using System.Collections.Concurrent;
using bstate.core.Middlewares;

namespace bstate.core.Services;

public interface IMiddlewareRegister
{
    public void AddPreprocessor<T>() where T : class, IPreProcessor;
    public void AddPostprocessor<T>() where T : class, IPostProcessor;
    public void AddBeaviour<T>() where T : class, IBehaviour;
    IEnumerable<Type> GetPreprocessors();
    IEnumerable<Type> GetPostprocessors();
    IEnumerable<Type> GetBehaviours();
}


class MiddlewareRegister : IMiddlewareRegister
{
    private readonly ConcurrentBag<Type> _preprocessors = new();
    private readonly ConcurrentBag<Type> _postprocessors = new();
    private readonly ConcurrentBag<Type> _beaviours = new();

    public void AddPreprocessor<T>() where T : class, IPreProcessor
    {
        _preprocessors.Add(typeof(T));
    }

    public void AddPostprocessor<T>() where T : class, IPostProcessor
    {
        _postprocessors.Add(typeof(T));
    }

    public void AddBeaviour<T>() where T : class, IBehaviour
    {
        _beaviours.Add(typeof(T));
    }

    public IEnumerable<Type> GetPreprocessors() => _preprocessors;

    public IEnumerable<Type> GetPostprocessors() => _postprocessors;
    public IEnumerable<Type> GetBehaviours() => _beaviours;
}