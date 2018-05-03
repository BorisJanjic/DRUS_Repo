using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ServiceModel;
using System.Runtime.Serialization;


namespace RTUClient
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    [DataContract]
    class Client : MeasurementServiceReference.IMeasurementServiceCallback
    {
        private int id;
        private MeasurementServiceReference.MeasurementServiceClient client;

        public Client(int id)
        {

            this.id = id;
      
            InstanceContext context = new InstanceContext(this);
            client = new MeasurementServiceReference.MeasurementServiceClient(context);
            StartMeasuring();
        }
        public int getId()
        {
            return id;
        }

        public void NotifyOfMeasurement(int id, double value, string type)
        {
            throw new NotImplementedException(); 
        }
        private void StartMeasuring()
        {
            int i = 0;
            while(true)
            {
                i++;
                Thread.Sleep(1000);//ms
                if(i%6==0)
                {
                    int humidity = GenerateMeasurement(40, 100);
                    DisplayMeasurement("Just measured humidity of " + humidity + " %. Sending results to monitors");
                    client.AddMeasurement(this.id, humidity, "Humidity");
                }
                int temperature = GenerateMeasurement(-13, 45);
                DisplayMeasurement("Just measured temperature of " + temperature+ " C. Sending results to monitors");
                client.AddMeasurement(this.id, temperature, "Temperature");
            }

        }
        private int GenerateMeasurement(int low, int high)
        {
            int numVar = new Random().Next(low, high);
            return numVar;
        }
        private void DisplayMeasurement(string status)
        {
            Console.WriteLine("Measurer (id = {0} ] : {1}", this.id, status);
        }
    }
}
