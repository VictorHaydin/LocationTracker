using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disruptor;
using ObjectsSimulator;

namespace LocationTracker
{
    public class ObjectPersistHandler : IEventHandler<TrackedObject>, IDisposable
    {
        private readonly StreamWriter _writer;

        public ObjectPersistHandler(string path)
        {
            _writer = new StreamWriter(path, true);
        }

        public void OnNext(TrackedObject data, long sequence, bool endOfBatch)
        {
            AppendData(data);
            _writer.Flush();
        }

        private void AppendData(TrackedObject data)
        {
            _writer.Write(data.Id);
            _writer.Write(' ');
            _writer.Write(data.Latitude);
            _writer.Write(' ');
            _writer.Write(data.Longitude);
            _writer.Write(Environment.NewLine);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
