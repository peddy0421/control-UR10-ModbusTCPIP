using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using System.IO;
using System.IO.Ports;


namespace WindowsFormsApplication41
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //=================全域變數宣告==================
        string[] type = { "RS232", "TCP" };
        Modbus modbus = new Modbus();


        string SetY0 = "01050500FF00";
        string ResetY0 = "010505000000";
        string SetY1 = "01050501FF00";
        string ResetY1 = "010505010000";
        string SetY2 = "01050502FF00";
        string ResetY2 = "010505020000";
        string SetY3 = "01050503FF00";
        string ResetY3 = "010505030000";
        string SetY4 = "01050504FF00";
        string ResetY4 = "010505040000";
        string SetY5 = "01050505FF00";
        string ResetY5 = "010505050000";
        string SetY6 = "01050506FF00";
        string ResetY6 = "010505060000";
        string SetY7 = "01050507FF00";
        string ResetY7 = "010505070000";
        string ReadY = "010205000008";
        string ReadX = "010204000008";




        //================視窗載入  初始宣告===============
        private void Form1_Load(object sender, EventArgs e)
        {
            ptb_00.BackColor = Color.Black;
            ptb_01.BackColor = Color.Black;
            ptb_02.BackColor = Color.Black;
            ptb_03.BackColor = Color.Black;
            ptb_04.BackColor = Color.Black;
            ptb_05.BackColor = Color.Black;
            ptb_06.BackColor = Color.Black;
            ptb_07.BackColor = Color.Black;
            pictureBox_PLC.BackColor = Color.Silver;
            gpb_Type_TCP.Enabled = false;
            gpb_RS232.Enabled = false;
            btn_Connect.Enabled = false;
            btn_Disconnect.Enabled = false;
            cmb_Type.Items.AddRange(type);
            timer1.Enabled = true;
            timer1.Interval = 100;
        }

        //============= ComPort_Name載入combobox ====================
        public void ComportName()
        {
            cmb_Comport.Items.Clear();
            cmb_Comport.Items.AddRange(SerialPort.GetPortNames());
        }

        //===================== Timer掃描內容 ==================================================================
        private void timer1_Tick(object sender, EventArgs e)
        {
            //============= Comport狀態 ==========================
            modbus.IsOpen();
            if (modbus.isopen)
            {
                pictureBox_PLC.BackColor = Color.GreenYellow;
            }
            else
            {
                pictureBox_PLC.BackColor = Color.Silver;
            }
            //====================== 按鈕權限 ========================
            if (cmb_Comport.Text != "" || txb_Tcp_Ip.Text != "" && txb_Tcp_Port.Text != "")
            {
                btn_Connect.Enabled = true;
                btn_Disconnect.Enabled = true;
            }
            else
            {
                btn_Connect.Enabled = false;
                btn_Disconnect.Enabled = false;
            }
        }

        //=================== 連線 =============================================================================
        private void btn_Connect_Click(object sender, EventArgs e)
        {
            if (cmb_Type.Text == "TCP")
            {
                modbus.Connert(cmb_Type.Text, txb_Tcp_Ip.Text, txb_Tcp_Port.Text);
            }
            else if (cmb_Type.Text == "RS232")
                modbus.Connert(cmb_Type.Text, cmb_Comport.Text);


        }

        //===============重置comport=================================================================================
        private void btn_Restart_Click(object sender, EventArgs e)
        {
            ComportName();
        }

        //====================TCP/RS232區域是否開啟==========================================================
        private void cmb_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            modbus.Disconnect();
            if (cmb_Type.Text == "TCP")
            {
                gpb_Type_TCP.Enabled = true;
                gpb_RS232.Enabled = false;
            }
            else if (cmb_Type.Text == "RS232")
            {
                ComportName();
                gpb_Type_TCP.Enabled = false;
                gpb_RS232.Enabled = true;
            }
        }

        //=================中斷連線===================================================================
        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            modbus.Disconnect();

        }

        //================= 命令輸出 ===============================================================
        private void btn_Command_Click(object sender, EventArgs e)
        {
            string _reply = "";
            string reply = "";

            btn_Connect.PerformClick();
            _reply = modbus.Command(rtb_Command.Text, cmb_Type.Text);

            if (txb_Tcp_Port.Text == "29999")
            {
                for (int i = 0; i < _reply.Length; i = i + 2)
                {
                    reply = reply + _reply.Substring(i, 2) + " ";
                }
                rtb_response.Text = HexStringToASCII(reply);
            }
            else
            {
                rtb_response.Text = _reply;
            }
        }

        //===================== 命令欄清除 =================== ==================================
        private void btn_Command_clear_Click(object sender, EventArgs e)
        {
            rtb_Command.Clear();
        }

        //======================= 視窗關閉 ============================================================
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            modbus.Disconnect();

            
        }
        //======================= string to ascii ============================================================
        public static string HexStringToASCII(string hexstring)
        {
            byte[] bt = HexStringToBinary(hexstring);
            string lin = "";
            for (int i = 0; i < bt.Length; i++)
            {
                lin = lin + bt[i] + " ";
            }

            string[] ss = lin.Trim().Split(new char[] { ' ' });
            char[] c = new char[ss.Length];
            int a;
            for (int i = 0; i < c.Length; i++)
            {
                a = Convert.ToInt32(ss[i]);
                c[i] = Convert.ToChar(a);
            }

            string b = new string(c);
            return b;
        }      
        public static byte[] HexStringToBinary(string hexstring)
        {

            string[] tmpary = hexstring.Trim().Split(' ');
            byte[] buff = new byte[tmpary.Length];
            for (int i = 0; i < buff.Length; i++)
            {
                buff[i] = Convert.ToByte(tmpary[i], 16);
            }
            return buff;
        }
        //=======================掃描============================================================
        private void btn_scanning_Click(object sender, EventArgs e)
        {
            btn_Connect.PerformClick();
            modbus.TCPIP_IO();
            string output_true = modbus.Command("010200100008", cmb_Type.Text);
            string current = modbus.Command("010301220006", cmb_Type.Text);
            string temperature = modbus.Command("0103012c0006", cmb_Type.Text);
            string angle = modbus.Command("0103010e0006", cmb_Type.Text);

            //=======================DO感測=============================================================
            string output_16 =  output_true.Substring(18, 2);
            string output_2 = (Convert.ToString((Convert.ToInt32(output_16, 16)), 2)).PadLeft(8,'0');
            if (output_2.Substring(0, 1) == "1")
            {
                ptb_07.BackColor = Color.Green;
            }
            else ptb_07.BackColor = Color.Black;
            if (output_2.Substring(1, 1) == "1")
            {
                ptb_06.BackColor = Color.Green;
            }
            else ptb_06.BackColor = Color.Black;
            if (output_2.Substring(2, 1) == "1")
            {
                ptb_05.BackColor = Color.Green;
            }
            else ptb_05.BackColor = Color.Black;
            if (output_2.Substring(3, 1) == "1")
            {
                ptb_04.BackColor = Color.Green;
            }
            else ptb_04.BackColor = Color.Black;
            if (output_2.Substring(4, 1) == "1")
            {
                ptb_03.BackColor = Color.Green;
            }
            else ptb_03.BackColor = Color.Black;
            if (output_2.Substring(5, 1) == "1")
            {
                ptb_02.BackColor = Color.Green;
            }
            else ptb_02.BackColor = Color.Black;
            if (output_2.Substring(6, 1) == "1")
            {
                ptb_01.BackColor = Color.Green;
            }
            else ptb_01.BackColor = Color.Black;
            if (output_2.Substring(7, 1) == "1")
            {
                ptb_00.BackColor = Color.Green;
            } 
            else ptb_00.BackColor = Color.Black;
            
            Base_mA.Text = Convert.ToString((Convert.ToInt32(current.Substring(18, 4), 16)) * 0.001) + "mA";
            Shoulder_mA.Text = Convert.ToString((Convert.ToInt32(current.Substring(22, 4), 16)) * 0.001 ) + "mA";
            Elbow_mA.Text = Convert.ToString((Convert.ToInt32(current.Substring(26, 4), 16)) * 0.001) + "mA";
            Wrist1_mA.Text = Convert.ToString((Convert.ToInt32(current.Substring(30, 4), 16)) * 0.001) + "mA";
            Wrist2_mA.Text = Convert.ToString((Convert.ToInt32(current.Substring(34, 4), 16)) * 0.001) + "mA";
            Wrist3_mA.Text = Convert.ToString((Convert.ToInt32(current.Substring(38, 4), 16)) * 0.001) + "mA";

            Base_TPC.Text = Convert.ToString((Convert.ToInt32(temperature.Substring(18, 4), 16)) ) + "度";
            Shoulder_TPC.Text = Convert.ToString((Convert.ToInt32(temperature.Substring(22, 4), 16))) + "度";
            Elbow_TPC.Text = Convert.ToString((Convert.ToInt32(temperature.Substring(26, 4), 16))) + "度";
            Wrist1_TPC.Text = Convert.ToString((Convert.ToInt32(temperature.Substring(30, 4), 16))) + "度";
            Wrist2_TPC.Text = Convert.ToString((Convert.ToInt32(temperature.Substring(34, 4), 16))) + "度";
            Wrist3_TPC.Text = Convert.ToString((Convert.ToInt32(temperature.Substring(38, 4), 16))) + "度";

            double Base_angle_math = (Convert.ToInt32(angle.Substring(18, 4), 16)) * 0.057295779513;
            double Shoulder_angle_math = (Convert.ToInt32(angle.Substring(22, 4), 16)) * 0.057295779513;
            double Elbow_angle_math = (Convert.ToInt32(angle.Substring(26, 4), 16)) * 0.057295779513;
            double Wrist1_angle_math = (Convert.ToInt32(angle.Substring(30, 4), 16)) * 0.057295779513;
            double Wrist2_angle_math = (Convert.ToInt32(angle.Substring(34, 4), 16)) * 0.057295779513;
            double Wrist3_angle_math = (Convert.ToInt32(angle.Substring(38, 4), 16)) * 0.057295779513;

            if (Base_angle_math > 180)
            {
                Base_angle.Text = ((Convert.ToInt32(angle.Substring(18, 4), 16)) * 0.057295779513) - 360 + "度";
            }
            else Base_angle.Text = ((Convert.ToInt32(angle.Substring(18, 4), 16)) * 0.057295779513)+ "度";
            if (Shoulder_angle_math > 180)
            {
                Shoulder_angle.Text = ((Convert.ToInt32(angle.Substring(22, 4), 16)) * 0.057295779513) - 360 + "度";
            }
            else Shoulder_angle.Text = ((Convert.ToInt32(angle.Substring(22, 4), 16)) * 0.057295779513) + "度";
            if (Elbow_angle_math > 180)
            {
                Elbow_angle.Text = ((Convert.ToInt32(angle.Substring(26, 4), 16)) * 0.057295779513) - 360 + "度";
            }
            else Elbow_angle.Text = ((Convert.ToInt32(angle.Substring(26, 4), 16)) * 0.057295779513) + "度";
            if (Wrist1_angle_math > 180)
            {
                Wrist1_angle.Text = ((Convert.ToInt32(angle.Substring(30, 4), 16)) * 0.057295779513) - 360 + "度";
            }
            else Wrist1_angle.Text = ((Convert.ToInt32(angle.Substring(30, 4), 16)) * 0.057295779513) + "度";
            if (Wrist2_angle_math > 180)
            {
                Wrist2_angle.Text = ((Convert.ToInt32(angle.Substring(34, 4), 16)) * 0.057295779513) - 360 + "度";
            }
            else Wrist2_angle.Text = ((Convert.ToInt32(angle.Substring(34, 4), 16)) * 0.057295779513) + "度";
            if (Wrist3_angle_math > 180)
            {
                Wrist3_angle.Text = ((Convert.ToInt32(angle.Substring(38, 4), 16)) * 0.057295779513) - 360 + "度";
            }
            else Wrist3_angle.Text = ((Convert.ToInt32(angle.Substring(38, 4), 16)) * 0.057295779513) + "度";
        }
    }



}

