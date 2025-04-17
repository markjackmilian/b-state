using bstate.core;

namespace bstate.web.example.Components.Features.RandomTest;

internal partial class RandomTestState
{
    record RunALongAction : IAction;
    public Task RunALong() => this.Channel.Send(new RunALongAction());

    record SetRandomNameAction : IAction;
    public Task SetRandomName() => this.Channel.Send(new SetRandomNameAction());
    
    record SetNameAction(string Name) : IAction;
    public Task SetName(string name) => this.Channel.Send(new SetNameAction(name));

    record SetIsLoadingAction(bool IsLoading) : IAction;
    public Task SetIsLoading(bool isLoading) => this.Channel.Send(new SetIsLoadingAction(isLoading));
}