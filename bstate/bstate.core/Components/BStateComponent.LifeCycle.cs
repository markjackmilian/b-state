using bstate.core.Services;

namespace bstate.core.Components;

public partial class BStateComponent
{
    protected override async Task OnInitializedAsync()
    {
        var instances = _onInitializes.Select(s => (IOnInitialize)ServiceProvider.GetService(s));
        foreach (var onInitialize in instances.Where(w=> w is not null))
        {
            await onInitialize.OnInitialize();
        }
        
        await base.OnInitializedAsync();
    }
}