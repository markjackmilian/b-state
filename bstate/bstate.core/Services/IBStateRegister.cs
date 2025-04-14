using bstate.core.Components;

namespace bstate.core.Services;

internal interface IBStateRegister
{
    void Add<T>(BStateComponent component) where T : BState;
    void Remove<T>(BStateComponent component) where T : BState;
    void Clear(BStateComponent component);
}

internal sealed class BStateRegister : IBStateRegister
{
     private readonly Dictionary<Type, HashSet<BStateComponent>> _stateComponents = new();
    
    public void Add<T>(BStateComponent component) where T : BState
    {
        var stateType = typeof(T);

        if (!_stateComponents.TryGetValue(stateType, out var components))
        {
            components = new HashSet<BStateComponent>();
            _stateComponents[stateType] = components;
        }

        components.Add(component);
    }

    public void Remove<T>(BStateComponent component) where T : BState
    {
        var stateType = typeof(T);

        if (_stateComponents.TryGetValue(stateType, out var components))
        {
            components.Remove(component);
            if (components.Count == 0)
            {
                _stateComponents.Remove(stateType);
            }
        }
    }

    public void Clear(BStateComponent component)
    {
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
}