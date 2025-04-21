using bstate.core.Classes;
using bstate.core.Services;
using bstate.web.example.Pages.Weather.Models;

namespace bstate.web.example.Features.Weather;

public partial class WeatherState
{
    record SetForecastAction(IEnumerable<WeatherForecast> Forecasts) : IAction;

    class SetForecastActionHandler(IStore store) : ActionHandler<SetForecastAction>(store)
    {
        private readonly IStore _store = store;
        WeatherState State => _store.Get<WeatherState>();
        public override Task Execute(SetForecastAction request)
        {
            this.State.Forecasts = request.Forecasts;
            return Task.CompletedTask;
        }
    }
    
    public Task SetForecasts(IEnumerable<WeatherForecast> forecasts) => this.ActionChannel.Send(new SetForecastAction(forecasts));
}
