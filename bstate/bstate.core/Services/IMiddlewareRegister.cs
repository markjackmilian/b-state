using System.Collections.Concurrent;
using bstate.core.Classes;
using bstate.core.Middlewares;

namespace bstate.core.Services;

public interface IMiddlewareRegister
{
    public void AddBehaviour<T>() where T : class, IBehaviour;
    IEnumerable<Type> GetBehaviours();
}

class MiddlewareRegister : IMiddlewareRegister
{
    private readonly ConcurrentBag<Type> _beaviours = new();

    public void AddBehaviour<T>() where T : class, IBehaviour
    {
        _beaviours.Add(typeof(T));
    }

    public IEnumerable<Type> GetBehaviours() => _beaviours;
}