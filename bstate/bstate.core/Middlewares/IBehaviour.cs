using bstate.core.Classes;
using PipelineNet.Middleware;

namespace bstate.core.Middlewares;

public interface IBehaviour : IAsyncMiddleware<IAction>
{ }