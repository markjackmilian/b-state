using bstate.core.Services;

namespace bstate.core;

public abstract class BState
{
    protected IActionChannel Channel { get; }

    protected BState(IActionChannel channel)
    {
        Channel = channel;
        this.Initialize();
    }
    protected abstract void Initialize();
}