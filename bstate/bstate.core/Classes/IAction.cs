using bstate.core.Services;

namespace bstate.core.Classes;

public interface IAction
{ }

public interface IActionHandler<in T> where T : IAction
{
    Task Execute(T request);
}

