using System;
using System.Threading;
using System.Threading.Tasks;
using Disruptor;
using ObjectsSimulator;

namespace LocationTracker
{
    class Program
    {
        // note: in this demo project it is important to keep ring buffer size twice bigger than objects count in order to avoid deadlocks when message publish itself into the same ring buffer
        //       in real-world application there will be additional network latency that actually will remove this risk
        private const int RingBufferSize = 1 << 11; // 2048
        private const int ObjectsCount = RingBufferSize / 2; 

        static void Main(string[] args)
        {
            var storageLocation = @"d:\test.log";
            if (args.Length > 0)
            {
                storageLocation = args[0];
            }
            var start = DateTime.Now;

            EventPublisher<TrackedObject> publisher = null; // will be initialized below, after ring buffer created
            var simulator = new Simulator(ObjectsCount, obj =>
            {
                publisher.PublishEvent((entry, sequenceNo) =>
                {
                    entry.Latitude = obj.Latitude;
                    entry.Longitude = obj.Longitude;
                    entry.Id = obj.Id;
                    return entry;
                });
            });

            var disruptor = new Disruptor.Dsl.Disruptor<TrackedObject>(() => new TrackedObject(), RingBufferSize, TaskScheduler.Default);
            
            var distanceHandler = new DistanceHandler();
            var aknowledgementHandler = new AknowledgementHandler(simulator);
            var consoleLogHandler = new ConsoleLogHandler();
            using (var persistHandler = new ObjectPersistHandler(storageLocation))
            {
                disruptor.HandleEventsWith(persistHandler).Then(consoleLogHandler, distanceHandler).Then(aknowledgementHandler);
                
                var ringBuffer = disruptor.Start();
                publisher = new EventPublisher<TrackedObject>(ringBuffer);
                
                simulator.PublishStartPositions();
                
                Thread.Sleep(10000);

                simulator.Stop();
                
                disruptor.Shutdown();
            }
            Console.WriteLine(distanceHandler.GetSummary());
            Console.WriteLine("Processed {0} events. Speed: {1:#.##} events per second.", simulator.MoveCount, simulator.MoveCount / (DateTime.Now - start).TotalSeconds);
            Console.ReadKey();
        }
    }
}
