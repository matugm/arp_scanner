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

            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            comboBox1.SelectedIndex = ArpScanner.deviceIndex;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selectedItem = (string)comboBox.SelectedItem;
            int selectedIndex = comboBox1.SelectedIndex;

            var devices = ArpScanner.getDeviceList();
            var selectedDevice = devices[selectedIndex];

            comboBox2.Items.Clear();
            comboBox2.ResetText();

            for (int i = 0; i < selectedDevice.Addresses.Count(); i++)
            {
                comboBox2.Items.Add(selectedDevice.Addresses.ElementAt(i).Addr);
            }

            ArpScanner.deviceIndex = selectedIndex;
            ArpScanner.deviceAddr = comboBox2.Items[1].ToString();
            comboBox2.SelectedIndex = 1;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
