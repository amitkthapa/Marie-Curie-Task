using System.Web.Mvc;
using InterviewTask.Services;
using System.Collections.Generic;
using InterviewTask.Models;
using InterviewTask.Helpers;
using System;
using System.Linq;

namespace InterviewTask.Controllers
{
    public class HomeController : Controller
    {
        private const string TemporarilyUnavailableStatus = "Temporarily unable to display";
        private const string RenderHelperInformationViewLocation = "/Views/Partials/HelperServiceInfo/_helperServiceInfo.cshtml";
        private const string OpenWeatherControllerUrl = "/OpenWeatherApi";
        private LogHelper LogHelper = new LogHelper();
        private List<int> ClosedTimes = new List<int> { 0, 0 };

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult RenderHelperServiceInformation()
        {
            var repo = new HelperServiceRepository();
            var vm = repo.Get();
            if (vm == null)
                return PartialView(RenderHelperInformationViewLocation, Enumerable.Empty<HelperServiceModel>());

            foreach (var helperServiceModel in vm)
            {
                var today = (int)DateTime.Now.DayOfWeek;
                helperServiceModel.TodayOpeningTimes = GetOpeningTimesToday(today, helperServiceModel);
                helperServiceModel.NextOpeningTimes = GetOpeningTimesNext(today, GetAllOpeningHours(helperServiceModel));
                CheckResponseFromServer(helperServiceModel);
                GetStatusMessage(helperServiceModel);
                helperServiceModel.WeatherInfoUrl = OpenWeatherControllerUrl;
            }

            return PartialView(RenderHelperInformationViewLocation, vm);
        }

        private OpeningTimes GetOpeningTimesToday(int today, HelperServiceModel model)
        {
            var openingTimesToday = new OpeningTimes();
            openingTimesToday.Day = today;
            switch (today)
            {
                case 1:
                    openingTimesToday.Time = model.MondayOpeningHours;
                    break;
                case 2:
                    openingTimesToday.Time = model.TuesdayOpeningHours;
                    break;
                case 3:
                    openingTimesToday.Time = model.WednesdayOpeningHours;
                    break;
                case 4:
                    openingTimesToday.Time = model.ThursdayOpeningHours;
                    break;
                case 5:
                    openingTimesToday.Time = model.FridayOpeningHours;
                    break;
                case 6:
                    openingTimesToday.Time = model.SaturdayOpeningHours;
                    break;
                case 0:
                    openingTimesToday.Time = model.SundayOpeningHours;
                    break;
            }
            return openingTimesToday;
        }

        private OpeningTimes GetOpeningTimesNext(int today, Dictionary<int, List<int>> allOpeningHours)
        {
            var openingTimesNext = new OpeningTimes();
            var hasNextOpeningHours = false;
            var currentDay = today;
            var nextDay = today == 0 ? 1 : currentDay + 1;
            var increment = 1;
            foreach (var openingHour in allOpeningHours)
            {
                if (hasNextOpeningHours) break;

                if (allOpeningHours[nextDay] == null)
                {
                    openingTimesNext.Time = null;
                    break;
                }
                else if (nextDay != currentDay && !allOpeningHours[nextDay].SequenceEqual(ClosedTimes))
                {
                    openingTimesNext.Time = allOpeningHours[nextDay];
                    openingTimesNext.Day = nextDay;
                    hasNextOpeningHours = true;
                    break;
                }

                if (nextDay == 6) nextDay = 0;
                else
                {
                    nextDay += increment;
                }
            }

            return openingTimesNext;
        }

        private Dictionary<int, List<int>> GetAllOpeningHours(HelperServiceModel model)
        {
            var openingHours = new Dictionary<int, List<int>>();
            openingHours.Add(1, model.MondayOpeningHours);
            openingHours.Add(2, model.TuesdayOpeningHours);
            openingHours.Add(3, model.WednesdayOpeningHours);
            openingHours.Add(4, model.ThursdayOpeningHours);
            openingHours.Add(5, model.FridayOpeningHours);
            openingHours.Add(6, model.SaturdayOpeningHours);
            openingHours.Add(7, model.SundayOpeningHours);

            return openingHours;
        }

        private void CheckResponseFromServer(HelperServiceModel model)
        {
            if (model.TodayOpeningTimes.Time == null || model.NextOpeningTimes.Time == null)
            {
                model.IsTemporarilyUnavailable = true;
                if (model.TodayOpeningTimes.Time == null)
                    LogHelper.LogError("Today Opening Times: null");
                if(model.NextOpeningTimes.Time == null)
                    LogHelper.LogError("Next Opening Times: null");
            }
        }

        private void GetStatusMessage(HelperServiceModel model)
        {
            if (model.IsTemporarilyUnavailable)
            {
                model.StatusMessage = TemporarilyUnavailableStatus;
                return;
            }

            var currentHour = DateTime.Now.Hour;
            if (currentHour <= model.TodayOpeningTimes.Time[0])
            {
                var openingTimeString = model.TodayOpeningTimes.Time[0].ToString();
                model.IsClosed = true;
                model.StatusMessage = $"Opens today at {openingTimeString}";
                return;
            }
            else if (currentHour >= model.TodayOpeningTimes.Time[1])
            {
                var tomorrowsOpeningTimeString = model.NextOpeningTimes.Time[0].ToString();
                model.IsClosed = true;
                var nextOpeningDay = Enum.GetName(typeof(DayOfWeek), model.NextOpeningTimes.Day);
                model.StatusMessage = $"Reopens {nextOpeningDay} at {tomorrowsOpeningTimeString}";
                return;
            }
            else
            {
                var todaysClosingTimeString = model.TodayOpeningTimes.Time[1];
                model.StatusMessage = $"Open today until {todaysClosingTimeString}";
                return;
            }
        }

    }
}