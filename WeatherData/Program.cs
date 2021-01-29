using System;

namespace WeatherDataApp
{
    class Program
    {
        static string filePath = @"C:\Users\Reko\source\repos\WeatherData\WeatherData\FileCSV\TemperaturData.csv";

        static void Main(string[] args)
        {
            //Kör bara denna första gången för att mata in datan i databasen.
            //ReadCSV.ReadCSVFile(filePath);
            WeatherMenu.Run();
        }
    }
}
