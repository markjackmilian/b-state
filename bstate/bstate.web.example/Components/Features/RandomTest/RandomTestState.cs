using bstate.core;
using bstate.core.Services;

namespace bstate.web.example.Components.Features.RandomTest;

internal partial class RandomTestState(IActionChannel channel) : BState(channel)
{
    public string Name { get; private set; }
    public bool IsLoading { get; private set; }

    protected override void Initialize()
    {
        this.Name = "Cavallo";
        this.IsLoading = false;
    }
}

internal partial class RandomTestState
{
    private string[] Names = new[] {"Cavallo", "Pippo", "Pluto", "Paperino", "Topolino", "Minnie", "Qui", "Quo", "Qua", "Rockerduck", "Gastone", "Paperoga", "Archimede", "Brigitta", "Basettoni", "Gambadilegno", "Macchia Nera", "Amelia", "Paperone", "Trudy"};
    private string GetRandomName() => Names[new Random().Next(0, Names.Length)];
    
    record RetrieveRandomNameAction : IAction;
    class RetrieveRandomNameActionHandler(IStore store) : ActionHandler<RetrieveRandomNameAction>(store)
    {
        RandomTestState State => store.Get<RandomTestState>();
        public override Task Execute(RetrieveRandomNameAction request)
        {
            State.Name = State.GetRandomName();
            return Task.CompletedTask;
        }
    }
    
    public Task SetRandomName() => this.Channel.Send(new RetrieveRandomNameAction());

}





internal partial class RandomTestState
{
    record SetNameAction(string Name) : IAction;

    class SetNameActionHandler(IStore store) : ActionHandler<SetNameAction>(store)
    {
        RandomTestState State => store.Get<RandomTestState>();
        public override Task Execute(SetNameAction request)
        {
            State.Name = request.Name;
            return Task.CompletedTask;
        }
    }

    public Task SetName(string name) => this.Channel.Send(new SetNameAction(name));
}

internal partial class RandomTestState
{
    record SetIsLoadingAction(bool IsLoading) : IAction;

    class SetIsLoadingActionHandler(IStore store) : ActionHandler<SetIsLoadingAction>(store)
    {
        private RandomTestState State => store.Get<RandomTestState>();
        public override Task Execute(SetIsLoadingAction request)
        {
            State.IsLoading = request.IsLoading;
            return Task.CompletedTask;
        }
    }

    public Task SetIsLoading(bool isLoading) => this.Channel.Send(new SetIsLoadingAction(isLoading));
}


internal partial class RandomTestState
{
    record RunALongAction : IAction;
    
    class RunALongActionHandler(IStore store) : ActionHandler<RunALongAction>(store)
    {
        RandomTestState State => store.Get<RandomTestState>();
        public override async Task Execute(RunALongAction request)
        {
            await State.SetIsLoading(true);
            await Task.Delay(3000);
            await State.SetRandomName();
            await State.SetIsLoading(false);
        }
    }

    public Task RunALong() => this.Channel.Send(new RunALongAction());
}




