using bstate.core.Services;

namespace bstate.core;

public abstract class BState(IActionBus actionChannel)
{
    protected IActionBus ActionChannel { get; } = actionChannel;

    private bool _isInitialized;

    protected abstract void Initialize();
    
    /// <summary>
    /// Called by UseState<T> from BstateComponent.
    /// </summary>
    public void InitializeIfNeeded()
    {
        if (_isInitialized) return;
        this.Initialize();
        _isInitialized = true;
    }
}