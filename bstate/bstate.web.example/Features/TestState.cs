using bstate.core;

namespace bstate.web.example.Features;

public class TestState : BState
{
    public string Name { get; private set; }
    public override void Initialize()
    {
        this.Name = "Cavallo";
    }
}