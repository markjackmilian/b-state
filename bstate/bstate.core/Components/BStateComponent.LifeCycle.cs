using System.Collections.Concurrent;
using bstate.core.Services;

namespace bstate.core.Components;

public partial class BStateComponent
{
    protected virtual void ConfigureHooks(){}

    protected override void OnInitialized()
    {
        this.ConfigureHooks();
        base.OnInitialized();
    }
    
    private readonly ConcurrentBag<Type> _onInitializes = new();
    
    protected void UseOnInitiaze<T>() where T : IOnInitialize
    {
        _onInitializes.Add(typeof(T));
    }
    protected override async Task OnInitializedAsync()
    {
        var instances = _onInitializes.Select(s => (IOnInitialize)ServiceProvider.GetService(s));
        foreach (var onInitialize in instances.Where(w=> w is not null))
        {
            await onInitialize.OnInitialize(this);
        }    
        await base.OnInitializedAsync();
    }
    
    private readonly ConcurrentBag<Type> _onAfterRenders = new();
    
    protected void UseOnAfterRenderAsync<T>() where T : IOnAfterRenderAsync
    {
        _onAfterRenders.Add(typeof(T));
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var instances = _onAfterRenders.Select(s => (IOnAfterRenderAsync)ServiceProvider.GetService(s));
        foreach (var onAfterRender in instances.Where(w => w is not null))
        {
            await onAfterRender.OnAfterRenderAsync(this, firstRender);
        }
        await base.OnAfterRenderAsync(firstRender);
    }
    
    private readonly ConcurrentBag<Type> _onBStateRenders = new();
    
    protected void UseOnBStateRender<T>() where T : IOnBStateRender
    {
        _onBStateRenders.Add(typeof(T));
    }
    
    private async Task InvokeOnBStateRender()
    {
        var instances = _onBStateRenders.Select(s => (IOnBStateRender)ServiceProvider.GetService(s));
        foreach (var onBStateRender in instances.Where(w => w is not null))
        {
            await onBStateRender.OnBStateRender(this);
        }
    }
    
    public async Task BStateRender()
    {
        await InvokeOnBStateRender();
        await InvokeAsync(StateHasChanged);
    }
}