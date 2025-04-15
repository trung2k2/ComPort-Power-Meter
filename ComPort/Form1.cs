using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComPort
{
    public partial class Form1 : Form
    {
        //public SerialPort serialPort1 = new SerialPort();
        private FormMain mainForm;
        public Form1(FormMain form)
        {
            InitializeComponent();
            mainForm = form;
            richTextBox1.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        public void AppendTextToRichTextBox2(string text)
        {
            if(mainForm.isWaitingForResponseInForm1 == true)
            {
                string extracted = text.Substring(12, 4) + " (A)";
                if (InvokeRequired)
                {

                    this.Invoke(new Action(() => richTextBox1.AppendText(extracted + Environment.NewLine)));
                }
                else
                {
                    richTextBox1.AppendText(extracted + Environment.NewLine);
                }
                mainForm.isWaitingForResponseInForm1 = false;
            }    
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (mainForm.serialPort1.IsOpen)
            {
                richTextBox1.Clear();
                // Tạo packet 01 52 31 02 31 2E 30 2E 31 31 2e 37 2e 30 28 29 03 65 
                List<byte> packet = new List<byte>();
                packet.Add(0x01);
                packet.Add(0x52);
                packet.Add(0x31);
                packet.Add(0x02);
                packet.Add(0x31);
                packet.Add(0x2e);
                packet.Add(0x30);
                packet.Add(0x2e);
                packet.Add(0x31);
                packet.Add(0x31);
                packet.Add(0x2e);
                packet.Add(0x37);
                packet.Add(0x2e);
                packet.Add(0x30);
                packet.Add(0x28);
                packet.Add(0x29);
                packet.Add(0x03);
                packet.Add(0x65);

                // Để debug hiển thị dạng chuỗi hex
                mainForm.serialPort1.Write(packet.ToArray(), 0, packet.Count);
                mainForm.isWaitingForResponseInForm1 = true;
                mainForm.countdown1 = 30;
            }
        }
    }
}
