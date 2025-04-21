using bstate.core;
using bstate.core.Services;

namespace bstate.web.example.Components.Features.RandomTest;

internal partial class RandomTestState(IActionBus actionChannel) : BState(actionChannel)
{
    public string Name { get; private set; }
    public bool IsLoading { get; private set; }

    protected override void Initialize()
    {
        this.Name = "Cavallo";
        this.IsLoading = false;
    }
}








