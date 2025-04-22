using bstate.core.Classes;

namespace bstate.web.example.Features.Counter;

public partial class CounterState
{
    record SetIsLoadingAction(bool IsLoading) : IAction;
    
    class SetIsLoadingActionHandler(CounterState counterState) : IActionHandler<SetIsLoadingAction>
    {
        public Task Execute(SetIsLoadingAction request)
        {
            counterState.IsLoading = request.IsLoading;
            return Task.CompletedTask;
        }
    }
    
    public Task SetIsLoading(bool isLoading) => this.ActionChannel.Send(new SetIsLoadingAction(isLoading));
}
