using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disruptor;
using ObjectsSimulator;

namespace LocationTracker
{
    public class ConsoleLogHandler : IEventHandler<TrackedObject>
    {
        public void OnNext(TrackedObject data, long sequence, bool endOfBatch)
        {
            if(sequence % 100000 == 0) Console.WriteLine("[{0}] {1}: {2}, {3}", sequence, data.Id, data.Latitude, data.Longitude);
        }
    }
}
