using System;
using System.Threading.Tasks;
using ClearToWork.Application.DTOs;
using ClearToWork.Application.Interfaces;

namespace ClearToWork.Infrastructure.Services;

public class WeatherService : IWeatherService
{
    public Task<WeatherForecastDto> GetForecastAsync(decimal latitude, decimal longitude, DateTime targetTime)
    {
        var forecast = new WeatherForecastDto(
            Latitude: latitude,
            Longitude: longitude,
            TemperatureC: 28.5,
            WindSpeedKmh: 12.0,
            WindGustsKmh: 18.0,
            PrecipitationProbability: 0.1,
            IsRainExpected: false,
            IsSafeForHotWork: true,
            IsSafeForHeightWork: true,
            Summary: "Clear skies, light winds; favorable for hot work and elevated maintenance."
        );
        return Task.FromResult(forecast);
    }
}
