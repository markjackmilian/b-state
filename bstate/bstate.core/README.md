# BState - State Management for Blazor Applications

BState is a lightweight state management library designed specifically for Blazor applications. It provides a clean and efficient way to manage application state with a middleware pipeline architecture.

## Features

- **Pipeline Architecture**: Uses a middleware pipeline for processing actions
- **Component Auto-Rendering**: Automatically re-renders components when state changes
- **Middleware Support**: Pre-processing and post-processing middleware capabilities
- **Type-Safe Actions**: Strongly-typed action handlers

## Getting Started

### 1. Define your state class

```csharp
public class CounterState : BState
{
    private int _count = 0;
    public int Count => _count;

    public CounterState(IActionChannel runner) : base(runner) { }
    
    protected override void Initialize()
    {
        // Initialize state if needed
    }
    
    // Define actions
    public record Increment : IAction;
    public record Decrement : IAction;
}
```

### 2. Create action handlers

```csharp
public class IncrementHandler : ActionHandler<CounterState.Increment>
{
    private readonly CounterState _state;
    
    public IncrementHandler(IStore store, CounterState state) : base(store)
    {
        _state = state;
    }
    
    public override async Task Execute(CounterState.Increment request)
    {
        // Update state
        _count++;
    }
}
```

### 3. Use state in components

```csharp
@inherits BStateComponent

<h1>Counter: @State.Count</h1>
<button @onclick="Increment">+</button>
<button @onclick="Decrement">-</button>

@code {
    private CounterState State;
    
    protected override void OnInitialized()
    {
        State = UseState<CounterState>();
    }
    
    private void Increment() => State.Runner.Send(new CounterState.Increment());
    private void Decrement() => State.Runner.Send(new CounterState.Decrement());
}
```

## Core Concepts

### BState

The base class for all state objects:

```csharp
public abstract class BState
{
    protected IActionChannel Runner { get; }

    protected BState(IActionChannel runner)
    {
        Runner = runner;
        this.Initialize();
    }
    protected abstract void Initialize();
}
```

### Actions and Handlers

Actions are simple marker interfaces:

```csharp
public interface IAction { }

public interface IActionHandler<in T> where T : IAction
{
    Task Execute(T request);
}

public abstract class ActionHandler<T>(IStore store) : IActionHandler<T> where T : IAction
{
    public abstract Task Execute(T request);
}
```

### BStateComponent

Components that use state inherit from BStateComponent:

```csharp
public abstract class BStateComponent : ComponentBase, IAsyncDisposable
{
    protected T UseState<T>() where T : BState
    {
        ComponentRegister.Add<T>(this);
        return ServiceProvider.GetService<T>()!;
    }

    public void BStateRender() => this.StateHasChanged();
    
    // ...
}
```

## Architecture

BState uses a pipeline architecture for processing actions through middleware:

```csharp
public async Task Send(IAction action)
{
    var pipeline = new PipelineBuilder(serviceProvider)
        .AddPreprocessors(configuration.MiddlewareRegister.GetPreprocessors())
        .AddActionRunner()
        .AddPostprocessors(configuration.MiddlewareRegister.GetPostprocessors())
        .AddRenderer()
        .Build();

    await pipeline.Execute(action);
}
```

## License

[MIT License](LICENSE)