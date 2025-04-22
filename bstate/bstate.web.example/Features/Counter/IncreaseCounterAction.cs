using bstate.core.Classes;
using bstate.core.Services;

namespace bstate.web.example.Features.Counter;

public partial class CounterState
{
    class IncreaseCounterActionHandler(CounterState counterState) : IActionHandler<IncreaseCounterAction>
    {
        public Task Execute(IncreaseCounterAction request)
        {
            counterState.Count++;
            return Task.CompletedTask;
        }
    }
    
    public Task IncreaseCounter() => this.ActionChannel.Send(new IncreaseCounterAction());
}
