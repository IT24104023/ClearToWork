using ClearToWork.Application.DTOs;

namespace ClearToWork.Application.Interfaces;

public interface IWeatherService
{
    Task<WeatherForecastDto> GetForecastAsync(decimal latitude, decimal longitude, DateTime targetTime);
}
