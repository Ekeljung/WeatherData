using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using WeatherDataApp.Models.Entities;

namespace WeatherDataApp
{
    public class ReadCSV
    {

        /// <summary>
        /// Read CSV file with data and add them to our database.
        /// </summary>
        /// <param name="filePath"></param>
        public static void ReadCSVFile(string filePath)
        {
            string[] resultSet = File.ReadAllLines(filePath);

            using (var context = new EfContext())
            {
                foreach (var data in resultSet)
                {
                    WeatherData2 wdInfo = new WeatherData2();
                    string[] wdString = data.Split(",");
                    wdInfo.DateAndTime = DateTime.Parse(wdString[0]);
                    wdInfo.Location = wdString[1];
                    wdInfo.Temp = decimal.Parse(wdString[2], CultureInfo.InvariantCulture);
                    wdInfo.Humidity = int.Parse(wdString[3], CultureInfo.InvariantCulture);

                    context.Add(wdInfo);
                }
                context.SaveChanges();
            }
        }
    }
}
