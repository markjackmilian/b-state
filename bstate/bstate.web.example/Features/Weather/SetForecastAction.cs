using bstate.core.Classes;
using bstate.core.Services;
using bstate.web.example.Pages.Weather.Models;

namespace bstate.web.example.Features.Weather;

public partial class WeatherState
{
    record SetForecastAction(IEnumerable<WeatherForecast> Forecasts) : IAction;

    class SetForecastActionHandler(WeatherState weatherState) : IActionHandler<SetForecastAction>
    {
        public Task Execute(SetForecastAction request)
        {
            weatherState.Forecasts = request.Forecasts;
            return Task.CompletedTask;
        }
    }
    
    public Task SetForecasts(IEnumerable<WeatherForecast> forecasts) => this.ActionChannel.Send(new SetForecastAction(forecasts));
}
