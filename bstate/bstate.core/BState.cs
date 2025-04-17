using bstate.core.Services;

namespace bstate.core;

public abstract class BState
{
    protected IBus Channel { get; }

    protected BState(IBus channel)
    {
        Channel = channel;
        this.Initialize();
    }
    protected abstract void Initialize();
}