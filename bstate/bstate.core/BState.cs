using bstate.core.Services;

namespace bstate.core;

public abstract class BState
{
    protected IActionBus ActionChannel { get; }

    protected BState(IActionBus actionChannel)
    {
        ActionChannel = actionChannel;
        this.Initialize();
    }
    protected abstract void Initialize();
}