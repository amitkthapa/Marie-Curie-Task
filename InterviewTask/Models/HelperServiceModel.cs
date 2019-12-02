using System;
using System.Collections.Generic;

namespace InterviewTask.Models
{
    public class HelperServiceModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TelephoneNumber { get; set; }
        public string StatusMessage { get; set; }

        public bool IsTemporarilyUnavailable { get; set; }
        public bool IsClosed { get; set; }

        public List<int> MondayOpeningHours { get; set; }
        public List<int> TuesdayOpeningHours { get; set; }
        public List<int> WednesdayOpeningHours { get; set; }
        public List<int> ThursdayOpeningHours { get; set; }
        public List<int> FridayOpeningHours { get; set; }
        public List<int> SaturdayOpeningHours { get; set; }
        public List<int> SundayOpeningHours { get; set; }

        public OpeningTimes TodayOpeningTimes { get; set; }
        public OpeningTimes NextOpeningTimes { get; set; }
        public string WeatherInfoUrl { get; set; }
    }
}

