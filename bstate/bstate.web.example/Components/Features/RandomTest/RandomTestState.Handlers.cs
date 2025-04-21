using bstate.core;
using bstate.core.Classes;
using bstate.core.Services;

namespace bstate.web.example.Components.Features.RandomTest;

internal partial class RandomTestState
{
    private string[] Names = new[] {"Cavallo", "Pippo", "Pluto", "Paperino", "Topolino", "Minnie", "Qui", "Quo", "Qua", "Rockerduck", "Gastone", "Paperoga", "Archimede", "Brigitta", "Basettoni", "Gambadilegno", "Macchia Nera", "Amelia", "Paperone", "Trudy"};
    private string GetRandomName() => Names[new Random().Next(0, Names.Length)];
    
    class SetRandomNameActionHandler(IStore store) : ActionHandler<SetRandomNameAction>(store)
    {
        private readonly IStore _store = store;
        RandomTestState State => _store.Get<RandomTestState>();
        public override Task Execute(SetRandomNameAction request)
        {
            State.Name = State.GetRandomName();
            return Task.CompletedTask;
        }
    }
}

internal partial class RandomTestState
{
    class SetNameActionHandler(IStore store) : ActionHandler<SetNameAction>(store)
    {
        private readonly IStore _store = store;
        RandomTestState State => _store.Get<RandomTestState>();
        public override Task Execute(SetNameAction request)
        {
            State.Name = request.Name;
            return Task.CompletedTask;
        }
    }

}

internal partial class RandomTestState
{
    class SetIsLoadingActionHandler(IStore store) : ActionHandler<SetIsLoadingAction>(store)
    {
        private readonly IStore _store = store;
        private RandomTestState State => _store.Get<RandomTestState>();
        public override Task Execute(SetIsLoadingAction request)
        {
            State.IsLoading = request.IsLoading;
            return Task.CompletedTask;
        }
    }

}

internal partial class RandomTestState
{
    class RunALongActionHandler(IStore store) : ActionHandler<RunALongAction>(store)
    {
        private readonly IStore _store = store;
        RandomTestState State => _store.Get<RandomTestState>();
        public override async Task Execute(RunALongAction request)
        {
            await State.SetIsLoading(true);
            await Task.Delay(3000);
            await State.SetRandomName();
            await State.SetIsLoading(false);
        }
    }

}