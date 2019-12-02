using InterviewTask.Services;
using System;
using System.Web.Configuration;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Http;
using InterviewTask.Models.Api;
using InterviewTask.Models.ViewModel;
using InterviewTask.Helpers;

namespace InterviewTask.Controllers
{
    public class OpenWeatherApiController : Controller
    {
        private const string HelperService = "Helper Service";
        private const string OpenWeatherApiBaseUrlKey = "OpenWeatherApiBaseUrl";
        private const string OpenWeatherAppIdKey = "OpenWeatherAppId";
        private const string WeatherInfoViewLocation = "/Views/Partials/Weather/_info.cshtml";
        private LogHelper _logHelper;

        public OpenWeatherApiController()
        {
            _logHelper = new LogHelper();
        }

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult RenderWeatherInfo([FromUri]Guid serviceId)
        {
            if (serviceId == Guid.Empty) return null;

            var requestedLocation = GetCurrentServiceLocation(serviceId);
            if (requestedLocation == null)
            {
                //Create Exception
                _logHelper.LogError("Location is null");
                return null;
            }

            var weatherInfo = GetWeather(requestedLocation);
            if (weatherInfo == null)
            {
                //Create Exception
                _logHelper.LogError("Weather info is null");
                return null;
            }
            var vm = new WeatherInfoViewModel();
            vm.AreaName = weatherInfo.Name;
            vm.CurrentTemp = ConvertKelvinToCelciusString(weatherInfo.Main.Temp);
            vm.Pressure = weatherInfo.Main.Pressure.ToString();
            vm.Humidity = weatherInfo.Main.Humidity.ToString();
            vm.MaxTemp = weatherInfo.Main.TempMax.ToString();
            vm.MinTemp = weatherInfo.Main.TempMin.ToString();
            vm.Description = weatherInfo.Weather[0].Description;

            return PartialView(WeatherInfoViewLocation, vm);
        }

        public string ConvertKelvinToCelciusString(double temp)
        {
            return (temp - 273.15).ToString();
        }
        public string GetCurrentServiceLocation(Guid serviceId)
        {
            var repo = new HelperServiceRepository();
            if (repo == null) return null;

            var currentService = repo.Get(serviceId);
            if (currentService == null) return null;

            var indexOfHelperService = currentService.Title.IndexOf(HelperService);
            var location = currentService.Title.Substring(0, indexOfHelperService);

            if (string.IsNullOrWhiteSpace(location)) return null;

            return location.Trim();
        }

        public WeatherInfo GetWeather(string areaName)
        {
            var baseUrl = WebConfigurationManager.AppSettings[OpenWeatherApiBaseUrlKey];
            var appId = WebConfigurationManager.AppSettings[OpenWeatherAppIdKey];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(appId))
            {
                return null;
            }

            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var urlParameters = $"?q={areaName}&APPID={appId}";

            var response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                var dataObjects = response.Content.ReadAsAsync<WeatherInfo>().Result;
                return dataObjects;
            }
            else
            {
                //Create exception
                var dataObjects = response.Content.ReadAsAsync<Error>().Result;
                _logHelper.LogError("Open Weather Api Error");
                _logHelper.LogError("Error Code : "+dataObjects.ErrorCode.ToString());
                _logHelper.LogError("Error Message: " + dataObjects.Message);
                return null;
            }
        }

    }
}