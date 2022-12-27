using System.Collections.Concurrent;
using System.Threading;
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

        private readonly TwitchClient _client;
        private readonly ConcurrentQueue<TwitchQueueMsg> _queue = new ConcurrentQueue<TwitchQueueMsg>();
        private readonly Thread _worker;

        public TwitchQueue(TwitchClient client)
        {
            _client = client;
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
                    if (_queue.Count > 0 && _queue.TryDequeue(out TwitchQueueMsg msg))
                        msg.Handler(_client, msg.Data);

                    Thread.Sleep(25);
                }
                catch (ThreadAbortException)
                {
                    return;
                }
            }
        }

        public void Enqueue(QueuedTwitchTaskHandler handler, object data) =>
            _queue.Enqueue(new TwitchQueueMsg { Handler = handler, Data = data });
    }
}
