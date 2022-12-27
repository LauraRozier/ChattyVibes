using Buttplug;
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

        private readonly ButtplugClientDevice _device;
        private readonly ConcurrentQueue<PlugQueueMsg> _queue = new ConcurrentQueue<PlugQueueMsg>();
        private readonly Thread _worker;

        public ButtplugDeviceQueue(ButtplugClientDevice device)
        {
            _device = device;
            _worker = new Thread(new ThreadStart(HandleQueue)) { IsBackground = true };
            _worker.Start();
        }

        public void Cleanup()
        {
            _worker.Abort();
            _worker.Join(5000);

            while (!_queue.IsEmpty)
                _queue.TryDequeue(out _);
        }

        private void HandleQueue()
        {
            while (true)
            {
                try
                {
                    if (_queue.Count > 0 && _queue.TryDequeue(out PlugQueueMsg msg))
                        msg.Handler(_device, msg.Data).Wait();

                    Thread.Sleep(25);
                }
                catch (ThreadAbortException)
                {
                    return;
                }
            }
        }

        public void Enqueue(QueuedPlugTaskHandler handler, object data) =>
            _queue.Enqueue(new PlugQueueMsg { Handler = handler, Data = data });
    }
}
