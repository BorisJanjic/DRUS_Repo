using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace MeasurementService
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class MeasurementService : IMeasurementService
    {
        private static Dictionary<int, List<IMeasurementServiceCallback>> observers = new Dictionary<int, List<IMeasurementServiceCallback>>();
        private static ProjectDatabaseMeasureEntities databaseEntities = new ProjectDatabaseMeasureEntities();
   
        public void Subscribe(int id)
        {
            IMeasurementServiceCallback callbackChanel = null;

            callbackChanel = OperationContext.Current.GetCallbackChannel<IMeasurementServiceCallback>();

            if (observers[id] == null)
            {
                observers[id] = new List<IMeasurementServiceCallback>();
            }
            observers[id].Add(callbackChanel);
        }
      
        public void Unsubscribe(int id)
        {
            IMeasurementServiceCallback callbackChanel = OperationContext.Current.GetCallbackChannel<IMeasurementServiceCallback>();
            observers[id].Remove(callbackChanel);
        }
      
        public void UnsubscribeAll(int id)
        {
            if (observers[id].Count != 0)
            {
                observers[id] = new List<IMeasurementServiceCallback>();
            }
        }
      
        public string ViewActiveStations()
        {
            string retVal = "";
            foreach (int i in observers.Keys)
            {
                retVal += i + " ";
            }
            if (retVal == "")
                retVal = "Empty!";
            return retVal;
        }
       
        public void AddMeasurement(int id, int value, string type)
        {
            if(observers.ContainsKey(id))
            {
                foreach(IMeasurementServiceCallback callbackChannel in observers[id] )
                {
                    callbackChannel.NotifyOfMeasurement(id, value, type);
                }
            }
            else
            {
                Console.WriteLine("No such key, creating...");
                observers.Add(id, new List<IMeasurementServiceCallback>());
            }
            SaveMeasurement(id, value, type, DateTime.Now);
        }

        public string GetAllMeasurementsOfMeasurer(int measurerId, DateTime fromTime, DateTime toTime, int type)
        {
            string typeStr = "";
            List<MEASUREMENT> measurements = new List<MEASUREMENT>();
            switch(type)
            {
                case 1:
                    typeStr = "Humidity";
                    measurements = (from m in databaseEntities.MEASUREMENTS
                                    where m.STATION.ID == measurerId &&
                                    m.TIME > fromTime && m.TIME < toTime &&
                                    m.TYPE.Equals(typeStr)
                                    select m).ToList<MEASUREMENT>();
                    break;
                case 2:
                    typeStr = "Temperature";
                    measurements = (from m in databaseEntities.MEASUREMENTS
                                    where m.STATION.ID == measurerId &&
                                    m.TIME > fromTime && m.TIME < toTime &&
                                    m.TYPE.Equals(typeStr)
                                    select m).ToList<MEASUREMENT>();
                    break;
                default:
                    measurements = (from m in databaseEntities.MEASUREMENTS
                                    where m.STATION.ID == measurerId &&
                                    m.TIME > fromTime && m.TIME < toTime
                                    select m).ToList<MEASUREMENT>();
                    break;
            }
            string retVal = "";

            foreach (MEASUREMENT m in measurements)
            {
                retVal += "[Id: " + m.ID + " Type: " + m.TYPE + " Value: " + m.VALUE + " ]\n";
            }
            return retVal;
        }
        
        public string GetAllMomentsOfLimitValues(int measurerId, int type, int limitType, double limit)
        {
            string typeStr = "";
            string retVal = "";
            List<MEASUREMENT> measurements = new List<MEASUREMENT>();
            if(type==1)
            {
                typeStr = "Humidity";
            }
            else if(type ==2)
            {
                typeStr = "Temperature";
            }
            //Low Limit
            if(limitType==1)
            {
                measurements = databaseEntities.MEASUREMENTS.Where(m => m.STATION.ID == measurerId
                                                && m.TYPE.Equals(typeStr) 
                                                && (double)m.VALUE < limit).ToList();
            }
            //High limit
            else if(limitType==2)
            {
                measurements = databaseEntities.MEASUREMENTS.Where(m => m.STATION.ID == measurerId
                                               && m.TYPE.Equals(typeStr)
                                               && (double)m.VALUE > limit).ToList();
            }
            retVal += "All moments of limit values for specific measurer: \n";
            foreach (MEASUREMENT m in measurements)
                retVal += "[Id: " + m.ID + " Type: " + m.TYPE + " Value: " + m.VALUE + " Time: " +m.TIME +" ]\n";
            return retVal;
        }
        
        public string GetAllMomentsOfLimitValuesOnSpecificLocation(string locationName, int type, int limitType, double limit)
        {
            string typeStr = "";
            string retVal = "";
            List<MEASUREMENT> measurements = new List<MEASUREMENT>();
            if (type == 1)
            {
                typeStr = "Humidity";
            }
            else if (type == 2)
            {
                typeStr = "Temperature";
            }
            //Low Limit
            if (limitType == 1)
            {
                measurements = databaseEntities.MEASUREMENTS.Where(m => m.STATION.LOCATION.NAME.Equals(locationName)                                                && m.TYPE.Equals(typeStr)
                                                && (double)m.VALUE < limit).ToList();
            }
            //High limit
            else if (limitType == 2)
            {
                measurements = databaseEntities.MEASUREMENTS.Where(m => m.STATION.LOCATION.NAME.Equals(locationName)
                                               && m.TYPE.Equals(typeStr)
                                               && (double)m.VALUE > limit).ToList();
            }
            retVal += "All moments of limit values on specific location:\n";
            foreach (MEASUREMENT m in measurements)
                retVal += "[Id: " + m.ID + " Type: " + m.TYPE + " Value: " + m.VALUE + " Time: " + m.TIME + " ]\n";
            return retVal;
        }

        public string GetAverageOnLocation(string locationName, int type, DateTime fromDate, DateTime dateUntil)
        {
            string typeStr = "";
            string retVal = "";
            List<MEASUREMENT> measurements = new List<MEASUREMENT>();
            if (type == 1)
            {
                typeStr = "Humidity";
            }
            else if (type == 2)
            {
                typeStr = "Temperature";
            }
            decimal averageResult = databaseEntities.MEASUREMENTS.Where(m => m.STATION.LOCATION.NAME.Equals(locationName)
                                        && m.TYPE.Equals(typeStr)
                                        && m.TIME < fromDate
                                        && m.TIME > dateUntil).Average(m=>m.VALUE);

            retVal = "[ Location name: " + locationName + " , Type: " + typeStr + " , " + "Average:" + averageResult + " ]";
            return retVal; 
        }
       
        private void SaveMeasurement(int id, double value, string type, DateTime currentTime)
        {
            MEASUREMENT measurement = new MEASUREMENT();
            STATION measuringStation = (from station in databaseEntities.STATIONS.ToList() where station.ID == id select station).FirstOrDefault();

            measurement.ID = id;
            measurement.VALUE = (decimal)value;
            measurement.TYPE = type;
            measurement.TIME = currentTime;

            databaseEntities.MEASUREMENTS.Add(measurement);
            databaseEntities.SaveChanges();

        }

    }
}
