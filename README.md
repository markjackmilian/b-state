![b-state logo](logo.png "Logo")


# BState - State Manager for Blazor

BState is a powerful state management library designed specifically for Blazor applications. It provides a clean, efficient way to manage application state through a flexible pipeline architecture.

## Key Features

- **Pipeline Architecture**: Process actions through customizable middleware pipelines
- **Pre/Post Processors**: Intercept and transform actions before and after processing
- **Behaviors**: Implement cross-cutting concerns with reusable behaviors
- **Automatic UI Updates**: Components automatically re-render when their state changes
- **Type-Safe Actions**: Strongly-typed action handlers for compile-time safety

## Getting Started

### Installation

```bash
dotnet add package b-state
```

### Register Services
Auto registration will register all actions and all lifecycle services. Preprocessors, PostProcessors, and Behaviors should be registered manually because their order matters
```csharp
// In Program.cs or Startup.cs
builder.Services.AddBState(configuration =>
{
    // Register handlers from the current assembly
    configuration.RegisterFrom(Assembly.GetExecutingAssembly());

    // Register behaviors
    configuration.AddBehaviour<ExceptionBehaviour>();
    configuration.AddBehaviour<LogBehaviour>();

    // Register preprocessors
    configuration.AddOpenRequestPreProcessor(typeof(AllRequestPreprocessor<>));
    configuration.AddOpenRequestPreProcessor(typeof(ALongActionPreProcessor<>));

    // Uncomment to register postprocessors
    // configuration.AddOpenRequestPostProcessor(typeof(TestPostprocessorMiddleware<>));
});

```

### Define Your State

```csharp
public partial class CounterState(IActionBus actionChannel) : BState(actionChannel)
{
    public int Count { get; private set; }
    public bool IsLoading { get; private set; }
    protected override void Initialize()
    {
        this.Count = 100; // init to 100 for some business rules :)
    }

    // Define your actions as nested types
    record IncreaseCounterAction : IAction;
    record DecreaseCounterAction : IAction, ILongAction;
    record SetIsLoadingAction(bool IsLoading) : IAction;
}
```

### Create Action Handlers

```csharp
public partial class CounterState
{
    class IncreaseCounterActionHandler(CounterState counterState) : IActionHandler<IncreaseCounterAction>
    {
        public Task Execute(IncreaseCounterAction request)
        {
            counterState.Count++;
            return Task.CompletedTask;
        }
    }

    public Task IncreaseCounter() => this.ActionChannel.Send(new IncreaseCounterAction());
}
```

### Use State in Components

```csharp
@using bstate.web.example.Features.Counter
@inherits bstate.core.Components.BStateComponent

<div class="card mt-2">
    <div class="card-body">
        <h5 class="card-title">Counter</h5>
        <p class="card-text" role="status">Current count: @State.Count</p>
        <button class="btn btn-primary" disabled="@State.IsLoading" @onclick="IncrementCount">Click me</button>
    </div>
</div>


@code {
    CounterState State => this.UseState<CounterState>();
    private Task IncrementCount() => this.State.IncreaseCounter();
}
```

## Advanced Features

### Preprocessors

Preprocessors run before action handlers and can modify or reject actions:

```csharp
class AllRequestPreprocessor<TAction> : IPreProcessor<TAction>
where TAction : IAction
{
    public Task Run(IAction parameter)
    {
        Console.WriteLine("AllRequestPreprocessor");
        return Task.CompletedTask;
    }
}
```

### Postprocessors

Postprocessors run after action handlers and can perform side effects:

```csharp
class AllRequestPostprocessor<TAction> : IPostProcessor<TAction>
where TAction : IAction
{
    public Task Run(IAction parameter)
    {
        Console.WriteLine("AllRequestPostprocessor");
        return Task.CompletedTask;
    }
}
```

### Behaviors

Behaviors implement cross-cutting concerns for specific action types:

```csharp
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
```

## Architecture

BState processes actions through a pipeline with these stages:

1. **Preprocessors**: Run in the order registered, can short-circuit the pipeline
2. **Behaviors**: Applied to specific action types
3. **Action Handler**: Executes the main logic for the action
4. **Postprocessors**: Run in the order registered after the action completes
5. **Renderer**: Updates the UI automatically for affected components

## Lifecycle Hooks

BState provides lifecycle hooks that allow you to execute code at specific points in a component's lifecycle. Implement these interfaces in your services to hook into the BStateComponent lifecycle:

### Available Lifecycle Interfaces

- **IOnInitialize**: Executes when a component is initialized
- **IOnParametersSet**: Executes when component parameters are set
- **IOnBStateRender**: Executes when a component is about to render from bstate library
- **IOnAfterRenderAsync**: Executes after a component has rendered
- **IOnDisposeAsync**: Executes when a component is being disposed


### Roslyn Analyzer
The bstate.analyzer project is a Roslyn Analyzer designed to enforce and strengthen the use of project conventions. It helps ensure consistency and adherence to coding standards across the workspace by analyzing code and providing actionable feedback during development.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

BState is licensed under the [MIT License](LICENSE).