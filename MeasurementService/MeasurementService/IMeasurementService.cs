using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MeasurementService
{
    
    [ServiceContract(CallbackContract = typeof(IMeasurementServiceCallback))]
    public interface IMeasurementService
    {
        [OperationContract(IsOneWay =true)]
        void Subscribe(int id);

        [OperationContract(IsOneWay =true)]
        void Unsubscribe(int id);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeAll(int id);

        [OperationContract]
        void AddMeasurement(int id, int value, string type);

        [OperationContract]
        String ViewActiveStations();

        [OperationContract]
        String GetAllMeasurementsOfMeasurer(int measurerId, DateTime fromTime, DateTime toTime, int type);
        [OperationContract]
        String GetAllMomentsOfLimitValues(int measurerId, int type, int limitType, double limit);

        [OperationContract]
        string GetAllMomentsOfLimitValuesOnSpecificLocation(String locationName, int type, int limitType, double limit);

        [OperationContract]
        String GetAverageOnLocation(String locationName, int type, DateTime dateFrom, DateTime dateUntil);
    }

    public interface IMeasurementServiceCallback
    {
        [OperationContract]
        void NotifyOfMeasurement(int id, double value, string type);
    }
    
}
