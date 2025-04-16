using bstate.core;
using bstate.core.Services;

namespace bstate.web.example.Components.Features.RandomTest;

internal partial class RandomTestState(IActionChannel runner) : BState(runner)
{
    public string Name { get; private set; }

    protected override void Initialize()
    {
        this.Name = "Cavallo";
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
    
    public Task SetRandomName() => this.Runner.Send(new RetrieveRandomNameAction());

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

    public Task SetName(string name) => this.Runner.Send(new SetNameAction(name));
}



