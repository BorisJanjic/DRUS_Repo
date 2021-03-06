﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using MonitorClient.MeasurementServiceReference;

namespace MonitorClient
{
    [CallbackBehavior(UseSynchronizationContext =false)]
    class MonitorClient : MeasurementServiceReference.IMeasurementServiceCallback
    {
        private MeasurementServiceReference.MeasurementServiceClient client;

        public MonitorClient()
        {
            InstanceContext context = new InstanceContext(this);
            client = new MeasurementServiceReference.MeasurementServiceClient(context);
            this.MonitorStation();
        }
        private void MonitorStation()
        {
            DisplayOptions();
            string answer = Console.ReadLine();
            int choice = int.Parse(answer);
            while (answer != "quit")
            {
                switch(answer)
                {
                    case "quit":
                        {
                            client.UnsubscribeAll(choice);
                           
                        }
                        break;
                    case "1":
                        {
                            client.Subscribe(choice);
                            
                        }
                        break;
                    case "2":
                        {
                            client.Unsubscribe(choice);
                            
                        }
                        break;
                    case "3":
                        {
                            StationMeasurement(0);
                        }
                        break;
                    case "4":
                        {
                            StationMeasurement(1);
                        }
                        break;
                    case "5":
                        {
                            Console.WriteLine("Available stations are: \n" + client.ViewActiveStations());
                            DisplayOptions();
                        }
                        break;
                    case "6":
                        {
                            LimitMoments(0);
                        }
                        break;
                    case "7":
                        {
                            LimitMoments(1);
                        }
                        break;
                    case "8":
                        {

                            LimitMoments(2);

                        }
                        break;
                    case "9":
                        {
                            DisplayOptions();

                        }
                        break;
                    default:
                        {
                            Console.WriteLine("Wrong choise!!!");
                            
                        }
                        break;




                }

            }

        }
        
        private void StationMeasurement(int i)
        {
            Console.WriteLine("Type in station id");

            String stationId = Console.ReadLine();
            int station = int.Parse(stationId);

            Console.WriteLine("Type in from date format [dd/mm/yyyy hh:mm:ss.stst]");

            String dateFromStr = Console.ReadLine();
            DateTime df = ConvertStringToDate(dateFromStr);

            Console.WriteLine("Type in until date format [dd/mm/yyyy hh:mm:ss.stst]");
            String dateUntilStr = Console.ReadLine();
            DateTime du = ConvertStringToDate(dateUntilStr);

            Console.WriteLine("Type in type of measurement \n 1=Humidity\n 2=Temperature\n 0=Both");
            String typeStr = Console.ReadLine();
            int type = int.Parse(typeStr);

            if (i==0)
                DisplayMeasurementsFromStation(station, df, du, 0);
            else
                DisplayMeasurementsFromStation(station, df, du, type);

            DisplayOptions();
        }

        private void LimitMoments(int i)
        {
            int station = 0;
            String location = "";
            double limit = 0.0;
            int limitSign = 0;

            if (i == 0)
            {
                Console.WriteLine("Type in station id");

                String stationId = Console.ReadLine();
                station = int.Parse(stationId);    
            }
            else
            {
                Console.WriteLine("Type in location name");

                location = Console.ReadLine();
            }
            //from
            Console.WriteLine("Type in type of measurement \n 1=Humidity\n 2=Temperature");

            String typeStr = Console.ReadLine();
            int type = int.Parse(typeStr);

            if (i < 2)
            {
                Console.WriteLine("Type in limit type \n 1 = < \n 2 ");

                String limitType = Console.ReadLine();
                limitSign = int.Parse(limitType);

                Console.WriteLine("Type in limit number");

                String limitNumber = Console.ReadLine();
                limit = double.Parse(limitNumber);
                if (i == 0)
                {
                    Console.WriteLine(client.GetAllMomentsOfLimitValues(station, type, limitSign, limit));
                }
                else
                {
                    Console.Write(client.GetAllMomentsOfLimitValuesOnSpecificLocation(location, type, limitSign, limit));
                }
            }
            else
            {
                Console.WriteLine("Type in from date (format [dd/mm/yyyy hh:mm:ss.stst])");
                String dateFromStr = Console.ReadLine();
                DateTime df = ConvertStringToDate(dateFromStr);
                //until
                Console.WriteLine("Type in until date (format [dd/mm/yyyy hh:mm:ss.stst])");
                String dateUntilStr = Console.ReadLine();
                DateTime du = ConvertStringToDate(dateUntilStr);

                DisplayAverages(type, location, df, du);
            }            
            DisplayOptions();
        }

        private void DisplayMeasurementsFromStation(int station, DateTime df, DateTime du, int v)
        {
            Console.Write(client.GetAllMeasurementsOfMeasurer(station, df, du, v));
        }

        public void NotifyOfMeasurement(int id, double value, string type)
        {
            DisplayMonitorStatus("From [" + id + "] just received [" + type + " =" + value + "] ");
        }

        private void DisplayAverages(int type, String locationName, DateTime dFrom, DateTime dUntil)
        {
            Console.Write(client.GetAverageOnLocation(locationName, type, dFrom, dUntil));
        }
        private void DisplayMonitorStatus(string status)
        {
            Console.WriteLine("[Monitor] : " + status);
        }
        private DateTime ConvertStringToDate(String strDate)
        {
            DateTime dt = Convert.ToDateTime(strDate);
            return dt;
        }
        private void DisplayOptions()
        {
            StringBuilder options = new StringBuilder();
            options.Append("Options:\n 1 - Subscribe \n");
            options.Append("2 - Unsubscribe\n");
            options.Append("3 - Get all measurements in certain period for a station\n");
            options.Append("4 - Get all measurements of a desired type in certain period for a station\n");
            options.Append("5 - Show available stations\n ");
            options.Append("6 - Get all moments of limit values\n");       
            options.Append("7 - Get all moments of limit values on specific location\n");
            options.Append("8 - Average temperature/humidity value from a specific location\n");
            options.Append("9 - View menu");
            options.Append("quit - exit monitor client\n");
            Console.WriteLine(options.ToString());
        }
    }
}
