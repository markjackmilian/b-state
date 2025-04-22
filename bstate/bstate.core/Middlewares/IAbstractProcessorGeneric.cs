using bstate.core.Classes;

namespace bstate.core.Middlewares;

public interface IAbstractProcessor
{
    Task Run(IAction parameter);
}

public interface IAbstractProcessorGeneric<in T> : IAbstractProcessor where T : notnull
{
}

public interface IPreProcessor<in T> :IAbstractProcessorGeneric<T> 
{ }

public interface IPostProcessor<in T> : IAbstractProcessorGeneric<T> 
{ }