using System;
using System.Collections.Generic;
using System.Linq;
using System.Spatial;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectsSimulator
{
    public class Simulator
    {
        private readonly Action<TrackedObject> _publish;
        private readonly Dictionary<Guid, TrackedObject> _objects = new Dictionary<Guid, TrackedObject>();
        private readonly ObjectsFactory _factory = new ObjectsFactory();
        private readonly Random _random = new Random((int) DateTime.Now.Ticks);
        private long _moveCount;
        private bool _isRunning;

        public Simulator(int count, Action<TrackedObject> publish)
        {
            _publish = publish;
            if (count <= 0)
            {
                throw new ArgumentException("count must be greater than 0", "count");
            }
            for (int i = 0; i < count; i++)
            {
                var obj = _factory.Create();
                _objects.Add(obj.Id, obj);
            }
        }

        public long MoveCount
        {
            get { return _moveCount; }
        }

        public void PublishStartPositions()
        {
            _isRunning = true;
            foreach (var trackedObject in _objects.Values)
            {
                _publish(trackedObject);
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void MoveObj(Guid id)
        {
            var obj = _objects[id];
            obj.Latitude += (_random.NextDouble() - 0.5) / 100;
            if (obj.Latitude < -180) obj.Latitude += 360;
            if (obj.Latitude > 180) obj.Latitude -= 360;
            obj.Longitude += (_random.NextDouble() - 0.5) / 100;
            if (obj.Longitude < -180) obj.Longitude += 360;
            if (obj.Longitude > 180) obj.Longitude -= 360;

            _moveCount++;

            if (_isRunning)
            {
                _publish(obj);
            }
        }
    }
}
