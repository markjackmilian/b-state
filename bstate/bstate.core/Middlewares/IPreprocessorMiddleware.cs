using bstate.core.Classes;
using PipelineNet.Middleware;

namespace bstate.core.Middlewares;

public interface IPreprocessorMiddleware : IAsyncMiddleware<IAction>
{ }