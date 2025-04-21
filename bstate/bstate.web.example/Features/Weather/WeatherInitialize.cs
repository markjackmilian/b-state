using System.Net.Http.Json;
using bstate.core.Components;
using bstate.core.Services;
using bstate.core.Services.Lifecycle;
using bstate.web.example.Pages.Weather.Models;

namespace bstate.web.example.Features.Weather;

class WeatherInitialize(HttpClient httpClient, WeatherState state) : IOnInitialize
{
    private WeatherState State => state;
    public async Task OnInitialize(BStateComponent component)
    {
        await Task.Delay(2000); // simulate async call ( fetch data from server)
        var forecasts = await httpClient.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json");
        await State.SetForecasts(forecasts);
    }
}