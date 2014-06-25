using System;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class MainAppWindow : Form
    {
        public MainAppWindow()
        {
            InitializeComponent();
            listView.View = View.Details;
        }

        private delegate void ArpScannerDelegate(MainAppWindow form);
        private delegate void formDelegate(string ip, string mac);

        private void button1_Click(object sender, EventArgs e)
        {
            if (ArpScanner.scanRunning)
            {
                MessageBox.Show("A scan is already in progress...");
                return;
            }

            ArpScanner scanner = new ArpScanner();
            ArpScannerDelegate scannerDelegate = new ArpScannerDelegate(scanner.scanNetwork);

            listView.Items.Clear();
            ProgressBar.Value = 0;

            IAsyncResult asyncResults = scannerDelegate.BeginInvoke(this,
                null, null);
        }

        internal void updateList(string ip, string mac)
        {
            if (listView.InvokeRequired)
            {
                formDelegate pointer = new formDelegate(updateList);
                Invoke(pointer, new object[] { ip, mac });
            }
            else
            {
                ListViewItem newItem = new ListViewItem(ip);
                newItem.SubItems.Add(mac);
                listView.Items.Add(newItem);
            }
        }

        private delegate void progressDelegate();

        internal void updateProgress()
        {
            if (listView.InvokeRequired)
            {
                progressDelegate pointer = new progressDelegate(updateProgress);
                Invoke(pointer);
            }
            else
            {
                this.ProgressBar.PerformStep();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form f = new Form2();
            f.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private delegate void spoofer(string host);

        private void BlockNetworkMenuItem_Click(object sender, EventArgs e)
        {
            int selectedItems = listView.SelectedItems.Count;
            for (int i = 0; i < selectedItems; i++)
            {
                var selected = listView.SelectedItems[i];
                var IPaddr   = selected.Text;
                selected.BackColor = System.Drawing.Color.Khaki;

                // Avoid adding the same IP twice.
                if (ArpScanner.threads.ContainsKey(IPaddr))
                {
                    continue;
                }

                Thread t = new Thread(() => ArpScanner.spoof(selected.Text));
                t.Start();
                ArpScanner.threads.Add(IPaddr, t);
                //selected.Selected = false;
            }

        }

        private void StopBlockingMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count <= 0)
                return;

            var selected = listView.SelectedItems[0];

            selected.BackColor = System.Drawing.Color.White;

            Thread t = (Thread)ArpScanner.threads[selected.Text];
            t.Abort();
            ArpScanner.threads.Remove(selected.Text);
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}