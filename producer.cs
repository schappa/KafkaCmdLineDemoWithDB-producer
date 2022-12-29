using Confluent.Kafka;
using System;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.IO;
using consumer.Models;
using System.Text.Json.Nodes;
using System.Text.Json;
using Newtonsoft.Json;

class Producer {
    static void Main(string[] args)
    {
        if (args.Length != 1) {
            Console.WriteLine("Please provide the configuration file path as a command line argument");
            return;
        }

        IConfiguration configuration = new ConfigurationBuilder()
            .AddIniFile(args[0])
            .Build();
        
        string topic = "PharmDemo";
        Console.Write($"Topic name used -- {topic}");
        int numProduced = 0;

        using (var producer = new ProducerBuilder<string, string>(
            configuration.AsEnumerable()).Build())
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(@"./TestData");
                FileInfo[] files = di.GetFiles("*.json");
                foreach (var item in files)
                {
                    Console.WriteLine($"Processing {item.Name}");

                    Guid key = Guid.NewGuid();

                    Shipment shipment = JsonConvert.DeserializeObject<Shipment>(File.ReadAllText(item.FullName));
                    shipment.PacketId = key;
                    shipment.PacketRcvd = DateTime.Now;
                    string msg = JsonConvert.SerializeObject(shipment);

                    producer.Produce(topic, new Message<string, string> { Key = key.ToString(), Value = msg },
                        (deliveryReport) =>
                        {
                            if (deliveryReport.Error.Code != ErrorCode.NoError)
                                Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                            else
                                Console.WriteLine($"Produced event to topic {topic}: key = {msg} value = {key}");
                        });

                    ++numProduced;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error!!!\r\nMessage:\t{ex.Message}\r\nStack:\t{ex.StackTrace}");
            }

            producer.Flush(TimeSpan.FromSeconds(10));
            Console.WriteLine($"{numProduced} messages were produced to topic {topic}");
        }
    }
}