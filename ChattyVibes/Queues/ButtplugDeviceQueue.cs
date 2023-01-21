using Buttplug.Client;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ChattyVibes.Queues
{
    public delegate Task QueuedPlugTaskHandler(ButtplugClientDevice device, object data);

    internal class ButtplugDeviceQueue
    {
        private struct PlugQueueMsg
        {
            public QueuedPlugTaskHandler Handler { get; set; }
            public object Data { get; set; }
        }

        private volatile bool _shouldStop = false;
        private readonly ButtplugClientDevice _device;
        private ConcurrentQueue<PlugQueueMsg> _queue = new ConcurrentQueue<PlugQueueMsg>();
        private readonly Thread _worker;

        public ButtplugDeviceQueue(ButtplugClientDevice device)
        {
            _device = device;
            _worker = new Thread(new ThreadStart(HandleQueue)) { IsBackground = true };
            _worker.Start();
        }

        public async Task Cleanup()
        {
            _shouldStop = true;

            while (_worker.IsAlive)
                await Task.Delay(10);
        }

        private void HandleQueue()
        {
            try
            {
                while (!_shouldStop)
                {
                    if (_queue.Count > 0 && _queue.TryDequeue(out PlugQueueMsg msg))
                        msg.Handler(_device, msg.Data).Wait();

                    Thread.Sleep(25);
                }
            }
            catch (ThreadAbortException)
            {
                _shouldStop = true;
            }
            finally
            {
                var tmp = _queue;
                _queue = null;

                while (!tmp.IsEmpty)
                    tmp.TryDequeue(out _);
            }
        }

        public void Enqueue(QueuedPlugTaskHandler handler, object data) =>
            _queue.Enqueue(new PlugQueueMsg { Handler = handler, Data = data });
    }
}
