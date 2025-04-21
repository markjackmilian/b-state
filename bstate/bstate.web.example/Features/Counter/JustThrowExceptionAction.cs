using bstate.core.Classes;

namespace bstate.web.example.Features.Counter;

public partial class CounterState
{
    record JustThrowExceptionAction : IAction;

    class JustThrowExceptionActionHandler(CounterState counterState) : IActionHandler<JustThrowExceptionAction>
    {
        public Task Execute(JustThrowExceptionAction request)
        {
            throw new Exception("Just a test exception");
        }
    }

    public Task JustThrow() => this.ActionChannel.Send(new JustThrowExceptionAction());
}