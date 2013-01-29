using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disruptor;
using ObjectsSimulator;

namespace LocationTracker
{
    public class DistanceHandler : IEventHandler<TrackedObject>
    {
        private readonly Dictionary<Guid, Tuple<double, double, double>> _registry = new Dictionary<Guid, Tuple<double, double, double>>(); // lat, long, dist
        private double _totalDistance = 0;

        public void OnNext(TrackedObject data, long sequence, bool endOfBatch)
        {
            Tuple<double, double, double> old;
            if (_registry.TryGetValue(data.Id, out old))
            {
                var distance = old.Item3 + GetDistance(data.Latitude, data.Longitude, old.Item1, old.Item2);
                _registry[data.Id] = new Tuple<double, double, double>(data.Latitude, data.Longitude, distance);
                _totalDistance += distance;
            }
            else
            {
                _registry.Add(data.Id, new Tuple<double, double, double>(data.Latitude, data.Longitude, 0));
            }
        }

        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees*Math.PI/180;
        }

        public string GetSummary()
        {
            var builder = new StringBuilder();
            foreach (var tuple in _registry)
            {
                builder.AppendFormat("{0}: {1}", tuple.Key, tuple.Value.Item3);
                builder.AppendLine();
            }
            builder.AppendFormat("Total distance travelled: {0}", _totalDistance);
            return builder.ToString();
        }
    }
}
