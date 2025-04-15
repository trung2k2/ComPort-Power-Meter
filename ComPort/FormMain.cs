using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace ComPort
{
    public partial class FormMain : Form
    {
        public SerialPort serialPort1 = new SerialPort();
        //string dataOUT;
        int packet_number = 0;
        char ETX = (char)0x03;
        public bool isWaitingForResponseInForm1 = false;
        //Timer timer1 = new Timer();
        public FormMain()
        {
            InitializeComponent();
            
        }
        Form1 form1;

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            cBoxCOMPORT.Items.AddRange(ports);
            form1 = new Form1(this);




        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cBoxCOMPORT.Text;
                serialPort1.BaudRate = Convert.ToInt32(CBoxBaudRate.Text);
                serialPort1.DataBits = Convert.ToInt32(cBoxDataBits.Text);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cBoxStopBits.Text);
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), cBoxParityBits.Text);
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived); 
                serialPort1.Open();
                progressBar1.Value = 100;
                
            }

            catch (Exception err)
            {
                MessageBox.Show(err.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        

        }

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string DataReceived = serialPort1.ReadExisting();

            //richTextBox1.AppendText(DataReceived + "\n");
            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(DataReceived );
                Console.WriteLine("DataReceived: " + DataReceived);
                Console.WriteLine("packet_number: " + packet_number);
                if (form1 != null && !form1.IsDisposed)
                {
                    form1.AppendTextToRichTextBox2(DataReceived);
                }
                List<byte> packet2 = new List<byte>();
                if (DataReceived.Contains((char)0x0A))
                {                   
                        packet2.Add(0x06);
                        packet2.Add(0x30);
                        packet2.Add(0x30);
                        packet2.Add(0x31);
                        packet2.Add(0x0D);
                        packet2.Add(0x0A);
                        serialPort1.Write(packet2.ToArray(), 0, packet2.Count);                    
                }
                else if (DataReceived.Contains(ETX) && packet_number == 0)
                {
                    //Nhap pass 01 50 31 02 28 6f 6d 6e 69 31 32 33 34 29 03 60

                    packet2 = new List<byte>();
                    packet2.Add(0x01);
                    packet2.Add(0x50);
                    packet2.Add(0x31);
                    packet2.Add(0x02);
                    packet2.Add(0x28);
                    packet2.Add(0x6f);
                    packet2.Add(0x6d);
                    packet2.Add(0x6e);
                    packet2.Add(0x69);
                    packet2.Add(0x31);
                    packet2.Add(0x32);
                    packet2.Add(0x33);
                    packet2.Add(0x34);
                    packet2.Add(0x29);
                    packet2.Add(0x03);
                    packet2.Add(0x60);

                    serialPort1.Write(packet2.ToArray(), 0, packet2.Count);
                    timer1.Tick += timer1_Tick;
                    timer1.Start();
                    packet_number = 1;
                }
            }));
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen)
            {
                serialPort1.Close();
                progressBar1.Value = 0;

            }
        }
        //private void btnSendData_Click(object sender, EventArgs e)
        //{
        //    if (serialPort1.IsOpen)
        //    {
        //        dataOUT = tBoxSerialNumber.Text;
        //        //serialPort1.WriteLine(dataOUT);
        //        serialPort1.Write(dataOUT);
        //    }
        //}

        private void btnSendData_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen)
            {
                string serialText = tBoxSerialNumber.Text;
                // Chuyển chuỗi số sang mảng byte dạng ASCII
                byte[] serialBytes = Encoding.ASCII.GetBytes(serialText);
                // Tạo packet đầy đủ
                List<byte> packet = new List<byte>();
                packet.Add(0x2F);
                packet.Add(0x3F);
                packet.AddRange(serialBytes); // Thêm các byte ASCII của chuỗi số
                packet.Add(0x21);
                packet.Add(0x0D);
                packet.Add(0x0A);

                // Để debug hiển thị dạng chuỗi hex
                //dataOUT = BitConverter.ToString(packet.ToArray()).Replace("-", " ");
                //MessageBox.Show(dataOUT);
                serialPort1.Write(packet.ToArray(), 0, packet.Count);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            cBoxCOMPORT.Items.Clear();
            cBoxCOMPORT.Items.AddRange(ports);
            
        }
        public int countdown1 = 30;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //MessageBox.Show("Bat timer");
            --countdown1;
            label10.Text = countdown1.ToString();
            //Setting.countdown = Setting.countdown - 1;
            //label8.Text = Setting.countdown.ToString(); // ví dụ bạn có Label tên là lblCountdown

            if (countdown1 <= 0)
            {
                string serialText = tBoxSerialNumber.Text;
                // Chuyển chuỗi số sang mảng byte dạng ASCII
                byte[] serialBytes = Encoding.ASCII.GetBytes(serialText);

                List<byte> packet = new List<byte>();
                packet.Add(0x2F);
                packet.Add(0x3F);
                packet.AddRange(serialBytes); // Thêm các byte ASCII của chuỗi số
                packet.Add(0x21);
                packet.Add(0x0D);
                packet.Add(0x0A);
                //packet_number = 0;
                // Để debug hiển thị dạng chuỗi hex
                //dataOUT = BitConverter.ToString(packet.ToArray()).Replace("-", " ");
                //MessageBox.Show(dataOUT);
                serialPort1.Write(packet.ToArray(), 0, packet.Count);
                //MessageBox.Show("Hết giờ!");
                countdown1 = 30;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //this.Hide(); // Ẩn Form1

            //mainForm.ShowDialog();
            if (form1.ShowDialog() == DialogResult.OK)
            //form1.Show();
            {
            //    //Status.status = true;
            //    //// Gọi phương thức để cập nhật lại dữ liệu trên FormMain
            //    //txtTenSP.Text = InfoOrder.TenSP;

            //    MessageBox.Show("Cập nhật thành công");
            }
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
    }

}
