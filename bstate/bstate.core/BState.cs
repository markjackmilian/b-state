using bstate.core.Services;

namespace bstate.core;

public abstract class BState
{
    protected IActionChannel Runner { get; }

    protected BState(IActionChannel runner)
    {
        Runner = runner;
        this.Initialize();
    }
    protected abstract void Initialize();
}