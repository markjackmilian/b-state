using System.Collections.Concurrent;
using bstate.core.Classes;
using bstate.core.Middlewares;

namespace bstate.core.Services;

public interface IBehaviourRegister
{
    public void AddBehaviour<T>() where T : class, IBehaviour;
    IEnumerable<Type> GetBehaviours();
}

class BehaviourRegister : IBehaviourRegister
{
    private readonly ConcurrentBag<Type> _beaviours = [];

    public void AddBehaviour<T>() where T : class, IBehaviour
    {
        _beaviours.Add(typeof(T));
    }

    public IEnumerable<Type> GetBehaviours() => _beaviours;
}