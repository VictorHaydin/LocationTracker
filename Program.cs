using System;
using System.Threading;
using System.Threading.Tasks;
using ObjectsSimulator;

namespace LocationTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            var start = DateTime.Now;
            int cnt = 0;
            var disruptor = new Disruptor.Dsl.Disruptor<TrackedObject>(() => new TrackedObject(), 1 << 10, TaskScheduler.Default);

            using (var persistHandler = new ObjectPersistHandler(@"d:\test.log"))
            {
                var distanceHandler = new DistanceHandler();
                disruptor.HandleEventsWith(persistHandler).Then(new ConsoleLogHandler()).Then(distanceHandler);

                var ringBuffer = disruptor.Start();

                var simulator = new Simulator(1 << 10);
                simulator.Start(obj =>
                                    {
                                        long sequenceNo = ringBuffer.Next();

                                        var entry = ringBuffer[sequenceNo];

                                        entry.Latitude = obj.Latitude;
                                        entry.Longitude = obj.Longitude;
                                        entry.Id = obj.Id;

                                        ringBuffer.Publish(sequenceNo);
                                        cnt++;
                                    }, 0);

                Thread.Sleep(10000);
                simulator.Stop();
                disruptor.Shutdown();
                Console.WriteLine(distanceHandler.GetSummary());
            }
            Console.WriteLine("Processed {0} events. Speed: {1:#.##} events per second.", cnt, cnt / (DateTime.Now - start).TotalSeconds);
        }
    }
}
