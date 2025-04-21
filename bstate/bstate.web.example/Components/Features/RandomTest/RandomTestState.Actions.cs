using bstate.core;
using bstate.core.Classes;

namespace bstate.web.example.Components.Features.RandomTest;

internal partial class RandomTestState
{
    record RunALongAction : IAction, IsLongAction;
    public Task RunALong() => this.ActionChannel.Send(new RunALongAction());

    record SetRandomNameAction : IAction;
    public Task SetRandomName() => this.ActionChannel.Send(new SetRandomNameAction());
    
    record SetNameAction(string Name) : IAction;
    public Task SetName(string name) => this.ActionChannel.Send(new SetNameAction(name));

    record SetIsLoadingAction(bool IsLoading) : IAction;
    public Task SetIsLoading(bool isLoading) => this.ActionChannel.Send(new SetIsLoadingAction(isLoading));
}