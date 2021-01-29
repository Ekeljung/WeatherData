using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherDataApp.Models.Entities
{
    class WeatherData2
    {
        public int Id { get; set; }
        public DateTime DateAndTime { get; set; }
        public string Location { get; set; }
        public decimal Temp { get; set; }
        public int Humidity { get; set; }
    }
}
