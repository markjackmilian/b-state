using bstate.core.Classes;
using bstate.core.Services;

namespace bstate.web.example.Features.Counter;

public partial class CounterState
{
    record IncreaseCounterAction : IAction;

    class IncreaseCounterActionHandler(IStore store) : ActionHandler<IncreaseCounterAction>(store)
    {
        private readonly IStore _store = store;
        private CounterState State => _store.Get<CounterState>();
        
        public override Task Execute(IncreaseCounterAction request)
        {
            this.State.Count++;
            return Task.CompletedTask;
        }
    }
    
    public Task IncreaseCounter() => this.ActionChannel.Send(new IncreaseCounterAction());
}
