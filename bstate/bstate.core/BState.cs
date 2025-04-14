namespace bstate.core;

public abstract class BState
{
    protected BState()
    {
        this.Initialize();
    }
    public abstract void Initialize();
}