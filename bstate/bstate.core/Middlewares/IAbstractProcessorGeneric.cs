using bstate.core.Classes;

namespace bstate.core.Middlewares;

public interface IAbstractProcessorGeneric<in T> : IAbstractProcessor where T : notnull
{
}

public interface IPreProcessorGeneric<in T> :IAbstractProcessorGeneric<T> 
{ }

public interface IPostProcessorGeneric<in T> : IAbstractProcessorGeneric<T> 
{ }