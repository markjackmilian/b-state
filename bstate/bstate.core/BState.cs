using bstate.core.Services;

namespace bstate.core;

public abstract class BState
{
    protected IActionBus Channel { get; }

    protected BState(IActionBus channel)
    {
        Channel = channel;
        this.Initialize();
    }
    protected abstract void Initialize();
}