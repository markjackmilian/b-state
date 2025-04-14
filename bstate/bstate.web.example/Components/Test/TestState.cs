using bstate.core;
using bstate.core.Services;
using PipelineNet.Middleware;

namespace bstate.web.example.Components.Test;

internal partial class TestState(IBstateRunner runner) : BState(runner)
{
    public string Name { get; private set; }

    protected override void Initialize()
    {
        this.Name = "Cavallo";
    }
}

internal partial class TestState
{
    public record TestAction(string Name) : IAction;

    public class TestActionHandler(IStore store) : ActionHandler<TestAction>(store)
    {
        TestState State => store.Get<TestState>();
        public override Task Execute(TestAction request)
        {
            State.Name = request.Name;
            return Task.CompletedTask;
        }
    }

    public Task SetName(string name) => this.Runner.Run(new TestAction(name));


}



