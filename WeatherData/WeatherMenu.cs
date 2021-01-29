using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WeatherDataApp.Models.Entities;

namespace WeatherDataApp
{
    class WeatherMenu
    {
        static bool run = true;

        public static void Run()
        {

            while (run)
            {
                MainMenu();
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Q:
                        Console.Clear();
                        bool run2 = true;
                        while (run2)
                        {
                            Console.WriteLine("Skriv in datum: yyyy-MM-dd");
                            try
                            {
                                DateTime date = Convert.ToDateTime(Console.ReadLine());
                                Console.Clear();
                                foreach (var item in SeekDate(date))
                                {
                                    Console.WriteLine(item);
                                }

                                PressToContinue();
                                run2 = false;
                            }
                            catch
                            {
                                Console.Clear();
                                Console.WriteLine("Du måste skriva in ett giltigt datum enligt modellen: yyyy-mm-dd");
                            }
                        }
                        break;

                    case ConsoleKey.W:
                        bool tempBool = true;
                        Console.Clear();

                        while (tempBool)
                        {
                            Console.WriteLine("Temperaturer.");
                            Console.WriteLine("1: Ute.");
                            Console.WriteLine("2: Inne.");
                            try
                            {
                                string InOrOutTemp = "";
                                int inputTemp = int.Parse(Console.ReadLine());
                                Console.Clear();
                                if (inputTemp == 1)
                                    InOrOutTemp = "Ute";
                                else if (inputTemp == 2)
                                    InOrOutTemp = "Inne";

                                foreach (var item in Temperatures(InOrOutTemp))
                                {
                                    Console.WriteLine(item);
                                }
                                PressToContinue();
                                tempBool = false;
                            }
                            catch
                            {
                                Console.Clear();
                                Console.WriteLine("Du måste skriva in ett giltigt värde.");
                            }
                        }
                        break;

                    case ConsoleKey.E:
                        bool humidityBool = true;
                        Console.Clear();
                        while (humidityBool)
                        {
                            Console.WriteLine("Luftfuktighet.");
                            Console.WriteLine("1. Ute.");
                            Console.WriteLine("2. Inne.");
                            try
                            {
                                string InOrOutHumidity = "";
                                int inputHumidity = int.Parse(Console.ReadLine());

                                if (inputHumidity == 1)
                                    InOrOutHumidity = "Ute";

                                else if (inputHumidity == 2)
                                    InOrOutHumidity = "Inne";

                                Console.Clear();
                                foreach (var item in HumidMethod(InOrOutHumidity))
                                {
                                    Console.WriteLine(item);
                                }
                                PressToContinue();
                                humidityBool = false;
                            }
                            catch
                            {
                                Console.Clear();
                                Console.WriteLine("Du måste skriva in ett giltigt värde.");
                            }
                        }
                        break;

                    case ConsoleKey.R:
                        Console.Clear();
                        for (int i = 0; i < MetrologicalFall().Count; i++)
                        {
                            if (i == 1)
                            {
                                Console.WriteLine($"Metrologisk höst började: {MetrologicalFall().First()} °C");
                                break;
                            }
                        }
                        PressToContinue();
                        break;

                    case ConsoleKey.T:
                        Console.Clear();
                        MetrologicalWinter();
                        PressToContinue();
                        break;

                    case ConsoleKey.Y:
                        bool moldLoop = true;
                        Console.Clear();
                        while (moldLoop)
                        {
                            Console.WriteLine("Mögel.");
                            Console.WriteLine("1. Ute.");
                            Console.WriteLine("2. Inne.");
                            try
                            {
                                string InOrOutMold = "";
                                int inputMold = int.Parse(Console.ReadLine());
                                if (inputMold == 1)
                                    InOrOutMold = "Ute";
                                else if (inputMold == 2)
                                    InOrOutMold = "Inne";

                                Console.Clear();
                                foreach (var item in Moldcheck(InOrOutMold))
                                {
                                    Console.WriteLine(item);
                                }
                                PressToContinue();
                                moldLoop = false;
                            }
                            catch
                            {
                                Console.Clear();
                                Console.WriteLine("Du måste skriva in ett giltigt värde.");
                            }
                        }
                        break;

                    case ConsoleKey.X:
                        run = false;
                        break;
                }
                Console.Clear();
            }
        }



        /// <summary>
        /// Use date to search for data.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static List<string> SeekDate(DateTime date)
        {
            using (var context = new EfContext())
            {
                List<string> seekDates = new List<string>();

                var outsideTemp = context.WeatherDataInfo
                    .Where(x => x.Location == "Ute" && x.DateAndTime.Date == date)
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, TempAvg = x.Average(x => x.Temp) });

                var insideTemp = context.WeatherDataInfo
                    .Where(x => x.Location == "Inne" && x.DateAndTime.Date == date)
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, TempAvg = x.Average(x => x.Temp) });

                foreach (var item in outsideTemp)
                {
                    seekDates.Add($"Medeltemperatur {item.DateAndTime.ToShortDateString()}:\n" +
                        $"Ute: {Math.Round(item.TempAvg, 1)}°C\n");
                }

                foreach (var item in insideTemp)
                {
                    seekDates.Add($"Medeltemperatur {item.DateAndTime.ToShortDateString()}:\n" +
                        $"Inne: {Math.Round(item.TempAvg, 1)}°C\n");
                }

                //Om ingen data hittas.
                if (seekDates.Count() == 0)
                {
                    seekDates.Add("Ingen data för detta datum.");
                }
                return seekDates;
            }
        }



        /// <summary>
        /// Show hottest and coldest temperature.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static List<string> Temperatures(string location)
        {
            using (var context = new EfContext())
            {
                List<string> avgTemps = new List<string>();

                //Visa resultat beroende på inne/ute, gruppera på datum. Visa medeltemp.
                var resultSet = context.WeatherDataInfo
                    .Where(x => x.Location == $"{location}")
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, AvgTemp = x.Average(x => x.Temp) })
                    .OrderByDescending(x => x.AvgTemp);

                var bottomResult = resultSet
                    .OrderBy(x => x.AvgTemp)
                    .Take(15)
                    .OrderBy(x => x.AvgTemp);

                var topResult = resultSet
                    .Take(15);

                InsideorOutSide(location);
                avgTemps.Add("Lägst temperatur");
                avgTemps.Add("--------------------------------");

                foreach (var result in bottomResult)
                {
                    avgTemps.Add($"{result.DateAndTime:yyyy/MM/dd} \t {Math.Round(result.AvgTemp, 1)} °C");
                }

                avgTemps.Add("\n");

                avgTemps.Add("Högst temperatur");
                avgTemps.Add("--------------------------------");

                foreach (var result in topResult)
                {
                    avgTemps.Add($"{result.DateAndTime:yyyy/MM/dd} \t {Math.Round(result.AvgTemp, 1)} °C");
                }
                return avgTemps;
            }
        }



        /// <summary>
        /// Calculate humidity.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static List<string> HumidMethod(string location)
        {
            using (var context = new EfContext())
            {
                List<string> avgHumidity = new List<string>();

                var resultSet = context.WeatherDataInfo
                    .Where(x => x.Location == $"{location}")
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, AVGHumid = x.Average(x => x.Humidity) });


                var topResult = resultSet
                    .OrderByDescending(x => x.AVGHumid)
                    .Take(15);

                var bottomResult = resultSet
                    .OrderBy(x => x.AVGHumid)
                    .Take(15)
                    .OrderByDescending(x => x.AVGHumid);

                //Välj mellan ute eller inne.
                InsideorOutSide(location);

                avgHumidity.Add("Torrast");
                avgHumidity.Add("--------------------------------");

                foreach (var result in bottomResult)
                {
                    avgHumidity.Add($"{result.DateAndTime:yyyy/MM/dd} \t {Math.Round(result.AVGHumid, 1)} %");
                }

                avgHumidity.Add("\n");

                avgHumidity.Add("Fuktigast");
                avgHumidity.Add("--------------------------------");

                foreach (var result in topResult)
                {
                    avgHumidity.Add($"{result.DateAndTime:yyyy/MM/dd} \t {Math.Round(result.AVGHumid, 1)} %");
                }
                return avgHumidity;
            }
        }



        /// <summary>
        /// Decides when the metrological winter starts.
        /// </summary>
        /// <returns></returns>
        private static bool MetrologicalWinter()
        {
            using (var context = new EfContext())
            {
                var q1 = context.WeatherDataInfo
                    .Where(x => x.Location == "Ute" && x.DateAndTime > DateTime.Parse("2016-08-01"))
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { day = x.Key, TempAvg = x.Average(x => x.Temp) })
                    .OrderBy(x => x.day);

                int turnCounter = 0;

                foreach (var weatherDataWinter in q1)
                {
                    if (weatherDataWinter.TempAvg <= 0)
                    {
                        turnCounter++;

                        if (turnCounter == 5)
                        {
                            Console.WriteLine(weatherDataWinter.day + " " + weatherDataWinter.TempAvg);
                            return true;
                        }
                    }
                    else
                    {
                        //Räkningen startar om.
                        turnCounter = 0;
                    }
                }
                //går vi ur loopen och turnCounter inte gått upp till 5 fanns det inga datum som uppfyllde vilkåren.
                //returneras false hittades inget datum.
                if (turnCounter == 0)
                {
                    Console.WriteLine("Inget värde hittades.");

                }
                return false;
            }
        }



        /// <summary>
        /// Decides when the metrological fall starts.
        /// </summary>
        /// <returns></returns>
        private static List<string> MetrologicalFall()
        {
            List<string> fallDates = new List<string>();
            using (var context = new EfContext())
            {
                var q2 = context.WeatherDataInfo
                    .Where(x => x.Location == "Ute" && x.DateAndTime > DateTime.Parse("2016-08-01"))
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { day = x.Key, tempAVG = x.Average(x => x.Temp) });

                var q3 = q2
                    .OrderBy(x => x.day);

                int metrologicalCounter = 0;

                foreach (var weatherDataFall in q3)
                {
                    if (weatherDataFall.tempAVG < 10)
                    {
                        metrologicalCounter++;
                        fallDates.Add($"{weatherDataFall.day:yyyy/MM/dd} \t {Math.Round(weatherDataFall.tempAVG, 1)}");
                        if (metrologicalCounter == 5)
                        {
                            return fallDates;
                        }
                    }
                    else
                    {
                        fallDates.Clear();
                        metrologicalCounter = 0;
                    }
                }
                return fallDates;
            }
        }



        /// <summary>
        /// Calculates mold.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private static List<string> Moldcheck(string location)
        {
            List<string> moldList = new List<string>();
            using (var context = new EfContext())
            {
                //Ta fram medelvärden och sortera från minst till störst.
                var q1 = context.WeatherDataInfo
                    .Where(x => x.Location == $"{location}")
                    .GroupBy(x => x.DateAndTime.Date)
                    .Select(x => new { DateAndTime = x.Key, HumAvg = x.Average(x => x.Humidity), TempAvg = x.Average(x => x.Temp) })
                    .OrderBy(x => ((x.HumAvg - 78)) * (double)(x.TempAvg / 15) / 0.22);

                //Endast 15 första resultaten syns.
                var topMold = q1
                    .Take(15);

                //Ta fram procent och sortera fallande.
                var bottomMold = q1
                    .OrderByDescending(x => ((x.HumAvg - 78)) * (double)(x.TempAvg / 15) / 0.22)
                    .Take(15);

                //Välj mellan ute eller inne.
                InsideorOutSide(location);
                moldList.Add("Minst mögelrisk");
                moldList.Add("------------------------------------------");

                foreach (var mold in topMold)
                {
                    //lägg till rätt typ av sträng i listan. 
                    if (Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22) <= 0)
                    {
                        //Eftersom vi får minus så väljer jag att inte visa det.
                        moldList.Add($"{mold.DateAndTime: yyyy/MM/dd} \t {Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22)}% (ingen risk)");
                    }
                }

                moldList.Add("\n");
                moldList.Add("Högst mögelrisk");
                moldList.Add("------------------------------------------");

                foreach (var mold in bottomMold)
                {
                    //kollar även där det ska vara högst risk för mögel om det finns minusvärden även där.
                    if (Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22) <= 0)
                    {
                        moldList.Add($"{mold.DateAndTime: yyyy/MM/dd} \t {Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22)}% (ingen risk)");
                    }
                    //Om risken för mögel är över 0%
                    else if (Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22) > 0)
                    {
                        moldList.Add($"{mold.DateAndTime: yyyy/MM/dd} \t {Math.Round((mold.HumAvg - 78) * (double)(mold.TempAvg / 15) / 0.22)}%");
                    }
                }
                return moldList;
            }
        }



        /// <summary>
        /// Just a method that tells us to continue in our menu.
        /// </summary>
        private static void PressToContinue()
        {
            Console.WriteLine();
            Console.WriteLine("Tryck för att fortsätta.");
            Console.ReadLine();
            Console.Clear();
        }



        /// <summary>
        /// Method to help us choose between inside/outside
        /// </summary>
        /// <param name="location"></param>
        private static void InsideorOutSide(string location)
        {
            if (location == "Ute")
            {
                Console.WriteLine("[Utomhus]");
            }
            else
            {
                Console.WriteLine("[Inomhus]");
            }
        }



        private static void MainMenu()
        {
            Console.WriteLine(@"  _      _________ ________ _________  ___  ___ _________ 
 | | /| / / __/ _ /_  __/ // / __/ _ \/ _ \/ _ /_  __/ _ |
 | |/ |/ / _// __ |/ / / _  / _// , _/ // / __ |/ / / __ |
 |__/|__/___/_/ |_/_/ /_//_/___/_/|_/____/_/ |_/_/ /_/ |_|");
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("Q");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("] Sök datum.");

            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("W");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("] Temperaturer.");

            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("E");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("] Luftfuktighet.");

            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("R");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("] Metrologisk höst.");

            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("T");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("] Metrologisk vinter.");

            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("Y");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("] Mögel.");

            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("X");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("] Avsluta.");
        }
    }
}