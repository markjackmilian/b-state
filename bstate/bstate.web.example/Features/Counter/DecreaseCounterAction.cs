using bstate.core.Classes;

namespace bstate.web.example.Features.Counter;

public partial class CounterState
{
    record DecreaseCounterAction : IAction;

    class DecreaseCounterActionHandler(CounterState counterState) : IActionHandler<DecreaseCounterAction>
    {
        public Task Execute(DecreaseCounterAction request)
        {
            counterState.Count--;
            return Task.CompletedTask;
        }
    }

    public Task DecreaseCounter() => this.ActionChannel.Send(new DecreaseCounterAction());
}