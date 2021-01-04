using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialCommunicate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                serialPort1.Open();
                button2.Enabled = false;//打开串口按钮不可用
                button3.Enabled = true;//关闭串口可用
            }
            catch 
            {
                MessageBox.Show("端口错误，请检查串口", "错误");
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                comboBox1.Items.Add("COM" + i.ToString());
            }
            comboBox1.Text = "COM1";//串口号默认值
            comboBox2.Text = "4800";//波特率默认值
            serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceived);
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!radioButton3.Checked)
            {
                string str = serialPort1.ReadExisting();//字符串方式读
                textBox1.AppendText(str);//添加内容
            }
            else
            {
                byte data;
                data = (byte)serialPort1.ReadByte();//强制数据类型转换
                string str = Convert.ToString(data, 16).ToUpper();//转化为大写十六进制
                textBox1.AppendText("Ox" + (str.Length == 1 ? "0" + str : str) + " ");//空位补零
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
                serialPort1.Close();//关闭串口
                button2.Enabled = true;//打开串口可用
                button3.Enabled = false;//关闭串口不可用
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] Data = new byte[1];
            if (serialPort1.IsOpen)
            { 
                if(textBox2.Text != " ")
                {
                    if (!radioButton1.Checked)
                    {
                        try
                        {
                            serialPort1.WriteLine(textBox2.Text);//写数据
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("串口数据写入错误", "错误");
                            serialPort1.Close();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < (textBox2.Text.Length - textBox2.Text.Length % 2) / 2; i++)
                        {
                            Data[0] = Convert.ToByte(textBox2.Text.Substring(i * 2, 2), 16);
                            serialPort1.Write(Data, 0, 1);//循环发送（输入字符0A0B0C，只发送0A,0B）
                        }
                        if(textBox2.Text.Length % 2 != 0)//剩下一位单独处理
                        {
                            Data[0] = Convert.ToByte(textBox2.Text.Substring(textBox2.Text.Length - 1,1),16);//例如0B，单独发送B
                            serialPort1.Write(Data, 0, 1);//发送
                        }
                    }
                }
            }
        }

    }
}
