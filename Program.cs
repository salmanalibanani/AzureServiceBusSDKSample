using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Configuration;

namespace AzureServiceBusSDKSample
{
  class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
          
            var connectionString = config["connectionString"];
            var client = new ManagementClient(connectionString);
            var queues = await client.GetQueuesAsync();
            
            Console.WriteLine("Queues");
            Console.WriteLine("");

            foreach (var queue in queues)
            {
                Console.WriteLine("Queue name: " + queue.Path);
                Console.WriteLine("Queue size in MB: " + queue.MaxSizeInMB);
                Console.WriteLine("Max delivery count: " + queue.MaxDeliveryCount.ToString());
                var qrti = await client.GetQueueRuntimeInfoAsync(queue.Path);
                Console.WriteLine("Current message count: " + qrti.MessageCount);
                Console.WriteLine("");
            }

            var topics = await client.GetTopicsAsync();

            Console.WriteLine("Topics");
            Console.WriteLine("");

            foreach (var topic in topics)
            {
                Console.WriteLine("Topic name: " + topic.Path);
                Console.WriteLine("Max size in MB: " + topic.MaxSizeInMB.ToString());
                Console.WriteLine("Default message time to live: " + topic.DefaultMessageTimeToLive.ToString());
                var trti = await client.GetTopicRuntimeInfoAsync(topic.Path);
                Console.WriteLine("Active message count: " + trti.MessageCountDetails.ActiveMessageCount.ToString());
                var subscriptions = await client.GetSubscriptionsAsync(topic.Path);
                if (subscriptions.Count == 0)
                    Console.WriteLine("No subscriptions");
            }

            var queueDescription = await client.GetQueueAsync("replyqueue");
            queueDescription.MaxDeliveryCount = 100;
            await client.UpdateQueueAsync(queueDescription);

            await client.CloseAsync();
            
            // Use QueueClient and TopicClient objects to send messages to queues and topics respectively.
            var queueClient = new QueueClient(connectionString, "replyqueue");  //second parameter is queue name.
            await queueClient.SendAsync(new Message() { Body = Encoding.ASCII.GetBytes("Hello queue again")});
            await queueClient.CloseAsync();

            var topicClient = new TopicClient(connectionString, "new-topic");  //second parameter is topic name
            await topicClient.SendAsync(new Message() {Body = Encoding.ASCII.GetBytes("Hello topic agian")});
            await topicClient.CloseAsync();
        }
    }
}


          
            
            