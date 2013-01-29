using System;
using System.Collections.Generic;
using System.Linq;
using System.Spatial;
using System.Text;
using System.Threading.Tasks;

namespace ObjectsSimulator
{
    public class ObjectsFactory
    {
        private static readonly Random _random = new Random();
        private static readonly ObjectsFactory _instance = new ObjectsFactory();

        public static ObjectsFactory Instance
        {
            get { return _instance; }
        }

        public TrackedObject Create()
        {
            return new TrackedObject
                       {
                           Id = Guid.NewGuid(),
                           Latitude = _random.NextDouble()* 360 - 180,
                           Longitude = _random.NextDouble()* 360 - 180
                       };
        }
    }
}
