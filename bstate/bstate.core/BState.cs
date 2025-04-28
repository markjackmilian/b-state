using bstate.core.Services;

namespace bstate.core;

public abstract class BState(IActionBus actionChannel)
{
    protected IActionBus ActionChannel { get; } = actionChannel;

}