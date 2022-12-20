using ST.Library.UI.NodeEditor;
using System;
using System.Threading;
using System.Windows.Forms;

namespace ChattyVibes.Nodes.DateTimeNode
{
    [STNode("/Time", "LauraRozier", "", "", "Clock node")]
    internal sealed class ClockNode : DateTimeNode
    {
        private Thread m_thread = null;
        private volatile bool _runThread;

        private STNodeOption m_op_out_time;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Clock";

            m_op_out_time = OutputOptions.Add("Time", typeof(DateTime), false);
        }

        // When the owner changed 
        protected override void OnOwnerChanged()
        {
            base.OnOwnerChanged();

            if (Owner == null)
            {
                _runThread = false;
                m_thread = null;
                return;
            }
            else if (m_thread == null)
            {
                _runThread = true;
                m_thread = new Thread(() => {
                    while (_runThread)
                    {
                        Thread.Sleep(1000);

                        if (_runThread)
                            BeginInvoke(new MethodInvoker(() => m_op_out_time?.TransferData(DateTime.Now)));
                    }
                }) { IsBackground = true };
                m_thread.Start();
            }
        }
    }
}
