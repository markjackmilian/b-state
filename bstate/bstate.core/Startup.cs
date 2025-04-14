using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq;
using bstate.core.Services;

namespace bstate.core;

public static class Startup
{
    public static void AddBState(this IServiceCollection serviceCollection)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        RegisterStates(serviceCollection, assemblies);
        serviceCollection.AddSingleton<IBStateRegister, BStateRegister>();
    }

    private static void RegisterStates(IServiceCollection serviceCollection, Assembly[] assemblies)
    {
        var bstateTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(BState)));

        foreach (var type in bstateTypes)
        {
            serviceCollection.AddSingleton(type);
        }
    }
}