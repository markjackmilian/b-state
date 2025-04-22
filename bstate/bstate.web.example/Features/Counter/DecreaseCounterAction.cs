using bstate.core.Classes;
using bstate.web.example.Classes;

namespace bstate.web.example.Features.Counter;

public partial class CounterState
{
    record DecreaseCounterAction : IAction, ILongAction;

    class DecreaseCounterActionHandler(CounterState counterState) : IActionHandler<DecreaseCounterAction>
    {
        public async Task Execute(DecreaseCounterAction request)
        {
            await counterState.SetIsLoading(true);
            await Task.Delay(2000);
            counterState.Count--;
            await counterState.SetIsLoading(false);
        }
    }

    public Task DecreaseCounter() => this.ActionChannel.Send(new DecreaseCounterAction());
}