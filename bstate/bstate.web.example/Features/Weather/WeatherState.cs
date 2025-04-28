using bstate.core;
using bstate.core.Classes;
using bstate.core.Services;
using bstate.web.example.Pages.Weather.Models;

namespace bstate.web.example.Features.Weather;

public partial class WeatherState(IActionBus actionChannel) : BState(actionChannel)
{
    public IEnumerable<WeatherForecast> Forecasts { get; private set; }

    record SetForecastAction(IEnumerable<WeatherForecast> Forecasts) : IAction;

}