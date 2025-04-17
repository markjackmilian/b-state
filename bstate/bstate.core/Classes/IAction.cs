using bstate.core.Services;

namespace bstate.core.Classes;

public interface IAction
{ }

public interface IActionHandler<in T> where T : IAction
{
    Task Execute(T request);
}

public abstract class ActionHandler<T>(IStore store) : IActionHandler<T> where T : IAction
{
    public abstract Task Execute(T request);
}

