using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using static ChattyVibes.FrmBindingGraphs;

namespace ChattyVibes
{
    public partial class FrmBindingGraphs : ChildForm
    {
        public static class Keyboard
        {
            private static readonly HashSet<Keys> _keys = new HashSet<Keys>();

            public static void OnKeyDown(Keys key)
            {
                if (!_keys.Contains(key))
                    _keys.Add(key);
            }

            public static void OnKeyUp(Keys key)
            {
                if (_keys.Contains(key))
                    _keys.Remove(key);
            }

            public static bool IsKeyDown(Keys key) =>
                _keys.Contains(key);

            public static bool IsAnyKeyDown
            {
                get { return _keys.Count > 0; }
            }
        }

        private readonly Timer _timer = new Timer();
        private int _timerDelayTick = 0;
        private const int C_TIMER_DELAY_THRESHOLD = 50;

        private readonly List<string> _graphFiles = new List<string>();
        private string _currentGraph = string.Empty;

        internal readonly static Color C_COLOR_BOOL = Color.FromArgb(255, 255, 109, 41);
        internal readonly static Color C_COLOR_FLOAT = Color.FromArgb(255, 103, 31, 211);
        internal readonly static Color C_COLOR_INT = Color.FromArgb(255, 32, 108, 255);
        internal readonly static Color C_COLOR_UINT = Color.FromArgb(255, 0, 0, 255);
        internal readonly static Color C_COLOR_STRING = Color.FromArgb(255, 128, 255, 128);
        internal readonly static Color C_COLOR_DATETIME = Color.FromArgb(255, 128, 255, 0);
        internal readonly static Color C_COLOR_COLOR = Color.FromArgb(255, 255, 238, 0);
        internal readonly static Color C_COLOR_IMAGE = Color.FromArgb(255, 0, 255, 238);

        internal readonly static Color C_COLOR_EVENT = Color.FromArgb(255, 255, 0, 255);
        internal readonly static Color C_COLOR_ACTION = Color.FromArgb(255, 255, 0, 0);
        internal readonly static Color C_COLOR_LOGIC = Color.FromArgb(255, 91, 127, 0);
        internal readonly static Color C_COLOR_BRANCH = Color.FromArgb(255, 147, 196, 13);
        internal readonly static Color C_COLOR_COMMENT = Color.FromArgb(255, 0, 170, 0);
        internal readonly static Color C_COLOR_HUB = Color.FromArgb(255, 128, 128, 128);

        public FrmBindingGraphs() : base()
        {
            KeyPreview = true;
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (Keyboard.IsKeyDown(keyData & ~Keys.Shift))
                return true;

            bool InitMove()
            {
                if (!Keyboard.IsAnyKeyDown)
                {
                    _timerDelayTick = 0;
                    _timer.Start();
                }

                Keyboard.OnKeyDown(keyData & ~Keys.Shift);
                return true;
            }

            switch (keyData)
            {
                case Keys.Up:
                case Keys.Shift | Keys.Up:
                    {
                        foreach (var node in nodeEditorPanel.Editor.GetSelectedNode())
                            node.Top -= 1;

                        return InitMove();
                    }
                case Keys.Down:
                case Keys.Shift | Keys.Down:
                    {
                        foreach (var node in nodeEditorPanel.Editor.GetSelectedNode())
                            node.Top += 1;

                        return InitMove();
                    }
                case Keys.Left:
                case Keys.Shift | Keys.Left:
                    {
                        foreach (var node in nodeEditorPanel.Editor.GetSelectedNode())
                            node.Left -= 1;

                        return InitMove();
                    }
                case Keys.Right:
                case Keys.Shift | Keys.Right:
                    {
                        foreach (var node in nodeEditorPanel.Editor.GetSelectedNode())
                            node.Left += 1;

                        return InitMove();
                    }
                default: return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    {
                        if ((e.Modifiers & Keys.Control) == Keys.Control)
                        {
                            foreach (STNode node in nodeEditorPanel.Editor.Nodes)
                                node.SetSelected(true, true);

                            e.Handled = true;
                        }

                        return;
                    }
                case Keys.S:
                    {
                        if ((e.Modifiers & Keys.Control) == Keys.Control && !string.IsNullOrWhiteSpace(_currentGraph))
                        {
                            nodeEditorPanel.Editor.SaveCanvas(Path.Combine(MainForm._graphDir, _currentGraph));
                            nodeEditorPanel.Editor.ShowAlert("Saved Canvas", Color.White, Color.FromArgb(125, Color.Green));
                            e.Handled = true;
                        }

                        return;
                    }
                case Keys.R:
                    {
                        if ((e.Modifiers & Keys.Control) == Keys.Control && !string.IsNullOrWhiteSpace(_currentGraph))
                        {
                            nodeEditorPanel.Editor.Nodes.Clear();
                            nodeEditorPanel.Editor.LoadCanvas(Path.Combine(MainForm._graphDir, _currentGraph));
                            nodeEditorPanel.Editor.ShowAlert("Reloaded Canvas", Color.White, Color.FromArgb(125, Color.Green));
                            e.Handled = true;
                        }

                        return;
                    }
                case Keys.Delete:
                    {
                        var nodes = nodeEditorPanel.Editor.GetSelectedNode();

                        foreach (STNode node in nodes)
                            nodeEditorPanel.Editor.Nodes.Remove(node);

                        e.Handled = true;
                        return;
                    }
                case Keys.Up:
                case Keys.Shift | Keys.Up:
                case Keys.Down:
                case Keys.Shift | Keys.Down:
                case Keys.Left:
                case Keys.Shift | Keys.Left:
                case Keys.Right:
                case Keys.Shift | Keys.Right:
                    {
                        Keyboard.OnKeyUp(e.KeyCode & ~Keys.Shift);
                        _timer.Stop();
                        e.Handled = true;
                        return;
                    }
                default:
                    {
                        base.OnKeyUp(e);
                        return;
                    }
            }
        }

        protected override void OnLoad(EventArgs ea)
        {
            base.OnLoad(ea);
            nodeEditorPanel.LoadAssembly(Application.ExecutablePath);
            nodeEditorPanel.Y = Height - 100;
            nodeEditorPanel.Editor.OptionConnected += new STNodeEditorOptionEventHandler((s, e) =>
            {
                string msg = "";

                switch (e.Status)
                {
                    case ConnectionStatus.NoOwner: msg = "No Owner"; break;
                    case ConnectionStatus.SameOwner: msg = "Same Owner"; break;
                    case ConnectionStatus.SameInputOrOutput: msg = "Both are input or output options"; break;
                    case ConnectionStatus.ErrorType: msg = "Different data types"; break;
                    case ConnectionStatus.SingleOption: msg = "Single link node"; break;
                    case ConnectionStatus.Loop: msg = "Circular path"; break;
                    case ConnectionStatus.Exists: msg = "Existing connection"; break;
                    case ConnectionStatus.EmptyOption: msg = "Blank option"; break;
                    case ConnectionStatus.Connected: msg = "Connected"; break;
                    case ConnectionStatus.DisConnected: msg = "Disconnected"; break;
                    case ConnectionStatus.Locked: msg = "Node is locked"; break;
                    case ConnectionStatus.Reject: msg = "Operation denied"; break;
                    case ConnectionStatus.Connecting: msg = "Being connected"; break;
                    case ConnectionStatus.DisConnecting: msg = "Disconnecting"; break;
                    case ConnectionStatus.InvalidType: msg = "Invalid data type"; break;
                }
                nodeEditorPanel.Editor.ShowAlert(
                    msg, Color.White,
                    e.Status == ConnectionStatus.Connected ? Color.FromArgb(125, Color.Green) : Color.FromArgb(125, Color.Red)
                );
            });
            nodeEditorPanel.Editor.CanvasScaled += new EventHandler((s, e) =>
            {
                nodeEditorPanel.Editor.ShowAlert(
                    nodeEditorPanel.Editor.CanvasScale.ToString("F2"),
                    Color.White, Color.FromArgb(125, Color.Yellow)
                );
            });
            nodeEditorPanel.Editor.NodeAdded += new STNodeEditorEventHandler((s, e) => e.Node.ContextMenuStrip = contextMenuStrip1);

            nodeEditorPanel.PropertyGrid.SetInfoKey("Author", "Mail", "Link", "Show Help");
            nodeEditorPanel.Editor.SetTypeColor(typeof(bool), C_COLOR_BOOL);
            nodeEditorPanel.Editor.SetTypeColor(typeof(float), C_COLOR_FLOAT);
            nodeEditorPanel.Editor.SetTypeColor(typeof(int), C_COLOR_INT);
            nodeEditorPanel.Editor.SetTypeColor(typeof(uint), C_COLOR_UINT);
            nodeEditorPanel.Editor.SetTypeColor(typeof(string), C_COLOR_STRING);
            nodeEditorPanel.Editor.SetTypeColor(typeof(DateTime), C_COLOR_DATETIME);
            nodeEditorPanel.Editor.SetTypeColor(typeof(Color), C_COLOR_COLOR);
            nodeEditorPanel.Editor.SetTypeColor(typeof(Image), C_COLOR_IMAGE);

            nodeEditorPanel.TreeView.PropertyGrid.SetInfoKey("Author", "Mail", "Link", "Show Help");
            nodeEditorPanel.TreeView.Editor.SetTypeColor(typeof(bool), C_COLOR_BOOL);
            nodeEditorPanel.TreeView.Editor.SetTypeColor(typeof(float), C_COLOR_FLOAT);
            nodeEditorPanel.TreeView.Editor.SetTypeColor(typeof(int), C_COLOR_INT);
            nodeEditorPanel.TreeView.Editor.SetTypeColor(typeof(uint), C_COLOR_UINT);
            nodeEditorPanel.TreeView.Editor.SetTypeColor(typeof(string), C_COLOR_STRING);
            nodeEditorPanel.TreeView.Editor.SetTypeColor(typeof(DateTime), C_COLOR_DATETIME);
            nodeEditorPanel.TreeView.Editor.SetTypeColor(typeof(Color), C_COLOR_COLOR);
            nodeEditorPanel.TreeView.Editor.SetTypeColor(typeof(Image), C_COLOR_IMAGE);

            contextMenuStrip1.ShowImageMargin = false;
            contextMenuStrip1.Renderer = new ToolStripRendererEx();

            foreach (var file in Directory.GetFiles(MainForm._graphDir, MainForm._graphFilePtrn, SearchOption.TopDirectoryOnly))
            {
                _graphFiles.Add(file);
                lbGraphs.Items.Add(Path.GetFileName(file));
            }

            _timer.Interval = 10;
            _timer.Tick += new EventHandler(Timer_Tick);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timer.Stop();
            _timer.Dispose();

            if (!string.IsNullOrEmpty(_currentGraph))
                nodeEditorPanel.Editor.SaveCanvas(Path.Combine(MainForm._graphDir, _currentGraph));

            nodeEditorPanel.Editor.Nodes.Clear();
            base.OnFormClosing(e);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!Keyboard.IsAnyKeyDown)
                return;

            // We wait (_timer.Interval * C_TIMER_DELAY_THRESHOLD) ms before we process button hold scroll
            if (_timerDelayTick < C_TIMER_DELAY_THRESHOLD)
            {
                _timerDelayTick++;
                return;
            }

            foreach (var node in nodeEditorPanel.Editor.GetSelectedNode())
            {
                if (Keyboard.IsKeyDown(Keys.Up))
                    node.Top -= 2;
                else if (Keyboard.IsKeyDown(Keys.Down))
                    node.Top += 2;

                if (Keyboard.IsKeyDown(Keys.Left))
                    node.Left -= 2;
                else if (Keyboard.IsKeyDown(Keys.Right))
                    node.Left += 2;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = "";
            var result = ShowInputDialogBox(ref name, "Enter the grap's name", "New Graph Name");

            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(_currentGraph))
                    nodeEditorPanel.Editor.SaveCanvas(Path.Combine(MainForm._graphDir, _currentGraph));

                nodeEditorPanel.Editor.Nodes.Clear();

                if (!name.EndsWith(MainForm._graphFileExt))
                    name += MainForm._graphFileExt;

                var path = Path.Combine(MainForm._graphDir, name);
                nodeEditorPanel.Editor.SaveCanvas(path);

                _graphFiles.Add(path);

                //_currentGraph = name;
                lbGraphs.SelectedIndex = lbGraphs.Items.Add(name);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show($"Are you sure you wish to delete {_currentGraph}?", "Confirm Deletion", MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
                return;

            File.Delete(Path.Combine(MainForm._graphDir, _currentGraph));
            _currentGraph = string.Empty;
            nodeEditorPanel.Editor.Nodes.Clear();
            lbGraphs.Items.RemoveAt(lbGraphs.SelectedIndex);
            lbGraphs.SelectedIndex = -1;
        }

        private void lbGraphs_SelectedIndexChanged(object sender, EventArgs e)
        {
            string curItem = (string)lbGraphs.SelectedItem;

            if (!string.IsNullOrEmpty(_currentGraph))
                nodeEditorPanel.Editor.SaveCanvas(Path.Combine(MainForm._graphDir, _currentGraph));

            nodeEditorPanel.Editor.Nodes.Clear();

            if (!string.IsNullOrEmpty(curItem))
            {
                _currentGraph = curItem;
                nodeEditorPanel.Editor.LoadCanvas(Path.Combine(MainForm._graphDir, curItem));
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nodeEditorPanel.Editor.ActiveNode == null)
                return;

            nodeEditorPanel.Editor.Nodes.Remove(nodeEditorPanel.Editor.ActiveNode);
        }

        private void lockLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nodeEditorPanel.Editor.ActiveNode == null)
                return;

            nodeEditorPanel.Editor.ActiveNode.LockLocation = !nodeEditorPanel.Editor.ActiveNode.LockLocation;
        }

        private static DialogResult ShowInputDialogBox(ref string input, string prompt, string title = "Title", int width = 300, int height = 200)
        {
            Size size = new Size(width, height);

            Form inputBox = new Form
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = size,
                Text = title
            };

            Label label = new Label
            {
                Text = prompt,
                Location = new Point(5, 5),
                Width = size.Width - 10
            };
            inputBox.Controls.Add(label);

            TextBox textBox = new TextBox
            {
                Size = new Size(size.Width - 10, 23),
                Location = new Point(5, label.Location.Y + 20),
                Text = input
            };
            inputBox.Controls.Add(textBox);

            Button okButton = new Button
            {
                DialogResult = DialogResult.OK,
                Name = "okButton",
                Size = new Size(75, 23),
                Text = "&OK",
                Location = new Point(size.Width - 80 - 80, size.Height - 30)
            };
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button
            {
                DialogResult = DialogResult.Cancel,
                Name = "cancelButton",
                Size = new Size(75, 23),
                Text = "&Cancel",
                Location = new Point(size.Width - 80, size.Height - 30)
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();

            if (result == DialogResult.OK)
                input = textBox.Text;

            return result;
        }
    }
}
