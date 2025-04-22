using bstate.core;
using bstate.core.Classes;
using bstate.core.Services;
using bstate.web.example.Classes;

namespace bstate.web.example.Features.Counter;

public partial class CounterState(IActionBus actionChannel) : BState(actionChannel)
{
    public int Count { get; private set; }
    public bool IsLoading { get; private set; }
    protected override void Initialize()
    {
        this.Count = 100; // init to 100 for some business rules :)
    }
    
    record IncreaseCounterAction : IAction;
    record DecreaseCounterAction : IAction, ILongAction;
    record SetIsLoadingAction(bool IsLoading) : IAction;
}