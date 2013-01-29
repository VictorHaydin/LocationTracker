using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disruptor;
using ObjectsSimulator;

namespace LocationTracker
{
    public class AknowledgementHandler : IEventHandler<TrackedObject>
    {
        private readonly Simulator _simulator;

        public AknowledgementHandler(Simulator simulator)
        {
            _simulator = simulator;
        }

        public void OnNext(TrackedObject data, long sequence, bool endOfBatch)
        {
            _simulator.MoveObj(data.Id);
        }
    }
}
