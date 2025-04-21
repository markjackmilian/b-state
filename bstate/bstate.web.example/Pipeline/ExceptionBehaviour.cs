using bstate.core.Classes;
using bstate.core.Middlewares;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace bstate.web.example.Pipeline;

class ExceptionBehaviour(IJSRuntime jsRuntime) : IBehaviour
{
    public async Task Run(IAction parameter, Func<IAction, Task> next)
    {
        try
        {
            await next(parameter);
        }
        catch (Exception ex)
        {
            var escapedMessage = ex.InnerException?.Message.Replace("'", "\\'").Replace("\"", "\\\"");
            var script = $"alert('Exception occurred: {escapedMessage}');";
            await jsRuntime.InvokeVoidAsync("eval", script);
        }
    }
}