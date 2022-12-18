using ST.Library.UI.NodeEditor;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ChattyVibes.Nodes.Time
{
    [STNode("/Time", "LauraRozier", "", "", "Clock node")]
    internal class ClockNode : STNode
    {
        private Thread m_thread = null;
        private STNodeOption m_op_out_time;
        private volatile bool _runThread;

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Clock";
            TitleColor = Color.FromArgb(200, FrmBindingGraphs.C_COLOR_DATETIME);
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
