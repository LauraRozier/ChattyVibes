using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;

namespace ChattyVibes.Queues
{
    public delegate void QueuedTwitchTaskHandler(TwitchClient _client, object data);

    internal class TwitchQueue
    {
        private struct TwitchQueueMsg
        {
            public QueuedTwitchTaskHandler Handler { get; set; }
            public object Data { get; set; }
        }

        private volatile bool _shouldStop = false;
        private readonly TwitchClient _client;
        private ConcurrentQueue<TwitchQueueMsg> _queue = new ConcurrentQueue<TwitchQueueMsg>();
        private readonly Thread _worker;

        public TwitchQueue(TwitchClient client)
        {
            _client = client;
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
                    if (_queue.Count > 0 && _queue.TryDequeue(out TwitchQueueMsg msg))
                        msg.Handler(_client, msg.Data);

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

        public void Enqueue(QueuedTwitchTaskHandler handler, object data) =>
            _queue.Enqueue(new TwitchQueueMsg { Handler = handler, Data = data });
    }
}
