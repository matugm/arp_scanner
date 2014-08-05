using System;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            foreach (var device in ArpScanner.getDeviceList())
            {
                Console.WriteLine(device.Description);
                comboBox1.Items.Add(device.Description);
            }

            comboBox1.SelectedIndex = ArpScanner.deviceIndex;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ComboBox comboBox   = (ComboBox)sender;
            string selectedItem = (string)comboBox.SelectedItem;
            int selectedIndex   = comboBox1.SelectedIndex;

            var devices = ArpScanner.getDeviceList();
            var selectedDevice = devices[selectedIndex];

            comboBox2.Items.Clear();
            comboBox2.ResetText();

            for (int i = 0; i < selectedDevice.Addresses.Count(); i++)
            {
                comboBox2.Items.Add(selectedDevice.Addresses.ElementAt(i).Addr);
            }

            ArpScanner.deviceIndex = selectedIndex;
            ArpScanner.deviceAddr  = comboBox2.Items[1].ToString();
            comboBox2.SelectedIndex = 1;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text = textBox.Text;
            int lastIP  = 0;

            bool ok = Int32.TryParse(text, out lastIP);

            if (ok && lastIP > 0 && lastIP < 255)
            {
                ArpScanner.maxRange = lastIP;
            }
        }
    }
}
