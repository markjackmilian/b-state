using bstate.core.Components;

namespace bstate.core.Services;

internal interface IComponentRegister : IDisposable
{
    void Add<T>(BStateComponent component) where T : BState;
    void Remove<T>(BStateComponent component) where T : BState;
    void Clear(BStateComponent component);

    BStateComponent[] GetComponents<T>() where T : BState;
    BStateComponent[] GetComponents(Type stateType);
    BStateComponent[] GetComponents();
}

internal sealed class ComponentRegister : IComponentRegister
{
    private readonly Dictionary<Type, HashSet<BStateComponent>> _stateComponents = new();
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);

    public void Add<T>(BStateComponent component) where T : BState
    {
        var stateType = typeof(T);

        _lock.EnterWriteLock();
        try
        {
            if (!_stateComponents.TryGetValue(stateType, out var components))
            {
                components = [];
                _stateComponents[stateType] = components;
            }

            components.Add(component);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Remove<T>(BStateComponent component) where T : BState
    {
        var stateType = typeof(T);

        _lock.EnterWriteLock();
        try
        {
            if (_stateComponents.TryGetValue(stateType, out var components))
            {
                components.Remove(component);
                if (components.Count == 0)
                {
                    _stateComponents.Remove(stateType);
                }
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Clear(BStateComponent component)
    {
        _lock.EnterWriteLock();
        try
        {
            // Create a copy of the dictionary entries to avoid modification during enumeration
            foreach (var entry in _stateComponents.ToList())
            {
                var components = entry.Value;
                if (components.Remove(component))
                {
                    if (components.Count == 0)
                    {
                        _stateComponents.Remove(entry.Key);
                    }
                }
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public BStateComponent[] GetComponents<T>() where T : BState => GetComponents(typeof(T));

    public BStateComponent[] GetComponents(Type stateType)
    {
        _lock.EnterReadLock();
        try
        {
            if (_stateComponents.TryGetValue(stateType, out var components))
            {
                return components.ToArray();
            }

            return [];
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public BStateComponent[] GetComponents()
    {
        _lock.EnterReadLock();
        try
        {
            // Create a new HashSet to avoid duplicate components
            var allComponents = new HashSet<BStateComponent>();

            // Iterate through all state types in the dictionary
            foreach (var componentSet in _stateComponents.Values)
            {
                // Add all components from each state type to our result set
                foreach (var component in componentSet)
                {
                    allComponents.Add(component);
                }
            }

            // Convert the HashSet to an array and return it
            return allComponents.ToArray();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void Dispose()
    {
        _lock.Dispose();
    }
}