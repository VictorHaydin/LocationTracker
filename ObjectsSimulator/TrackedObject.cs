using System;
using System.Collections.Generic;
using System.Linq;
using System.Spatial;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsSimulator
{
    public sealed class TrackedObject
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid Id { get; set; }
    }
}
