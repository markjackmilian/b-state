using bstate.core.Services;

namespace bstate.core;

public abstract class BState
{
    protected IBstateRunner Runner { get; }

    protected BState(IBstateRunner runner)
    {
        Runner = runner;
        this.Initialize();
    }
    protected abstract void Initialize();
}