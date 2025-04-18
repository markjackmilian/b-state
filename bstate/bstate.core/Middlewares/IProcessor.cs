using bstate.core.Classes;
using PipelineNet.Middleware;

namespace bstate.core.Middlewares;

public interface IAbstractProcessor
{
    Task Run(IAction parameter);
}

public interface IPreProcessor : IAbstractProcessor;
public interface IPostProcessor : IAbstractProcessor;