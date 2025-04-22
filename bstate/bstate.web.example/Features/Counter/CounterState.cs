using bstate.core;
using bstate.core.Services;

namespace bstate.web.example.Features.Counter;

public partial class CounterState(IActionBus actionChannel) : BState(actionChannel)
{
    public int Count { get; private set; }
    public bool IsLoading { get; private set; }
    protected override void Initialize()
    {
        this.Count = 100; // init to 100 for some business rules :)
    }
}