using bstate.core.Services;

namespace bstate.core.Classes;

public interface IRequest<T>
{ }

public interface IRequestHandler<in T, TR> where T : IRequest<TR>
{
    Task<TR> Handle(T request);
}

