using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
using System.Data.SQLite;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TrafficLightControlN6
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private int timeLeft;
        private bool isNorthSouthGreen;
        private bool isNightMode;
        private bool isManualMode;
        private SerialPort serialPort;
        private bool manualToggleState;
        private int timeTrafficLight1;
        private int timeTrafficLight2;
        private SQLiteConnection sqliteConn;
        private StringBuilder serialDataBuffer = new StringBuilder();

        public Form1()
        {
            InitializeDatabase();
            InitializeComponent();
            InitializeSerialPort();
            InitializeTrafficLight();
        }

        private void InitializeTrafficLight()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;

            SetTrafficTimes();
            timer.Start();
        }

        private void InitializeSerialPort()
        {
            serialPort = new SerialPort("COM2", 9600);
            serialPort.DataReceived += SerialPort_DataReceived;
            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cổng COM: {ex.Message}");
            }
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadLine();

            data = data.Replace("\n", "").Replace("\r", "");




            if (data != "END")
            {
                serialDataBuffer.Append(data);
            }

            // validate it it JSON format like {"counter1":24,"counter2":28,"light1":"green","light2":"red"} and ends with "END"
            if (data == "END" && serialDataBuffer.ToString().Trim().StartsWith("{") && serialDataBuffer.ToString().Trim().EndsWith("}"))
            {
                string fullData = serialDataBuffer.ToString().Trim();
                serialDataBuffer.Clear();

                Invoke(new Action(() => ProcessSerialData(fullData)));
            }
            else
            {
                // remove {Normal mode activated.{"counter1":0,"counter2":0,"light1":"red","light2":"yellow"} 
                int startIdx = serialDataBuffer.ToString().IndexOf("{");
                int endIdx = serialDataBuffer.ToString().LastIndexOf("}");

                if (startIdx >= 0 && endIdx >= 0 && startIdx < endIdx)
                {
                    // extract only the valid JSON content between the `{` and `}`
                    string validJson = serialDataBuffer.ToString().Substring(startIdx, endIdx - startIdx + 1);
                    serialDataBuffer.Clear();

                    serialDataBuffer.Append(validJson);
                }
            }
        }

        private void ProcessSerialData(string jsonData)
        {
            textBox1.Text += jsonData;

            try
            {
                // {"counter1":24,"counter2":28,"light1":"green","light2":"red"}
                var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(jsonData);

                int counter1 = jsonObject["counter1"].Value<int>();
                int counter2 = jsonObject["counter2"].Value<int>();
                string light1 = jsonObject["light1"].Value<string>();
                string light2 = jsonObject["light2"].Value<string>();


                timeTrafficLight1 = counter1;
                timeTrafficLight2 = counter2;

                northSouthLight.BackColor = light1 == "green" ? Color.Green : Color.Red;
                eastWestLight.BackColor = light2 == "green" ? Color.Green : Color.Red;

                UpdateTrafficLights();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing JSON: {ex.Message}");
            }

        }

        private void SendToArduino(char command)
        {

            if (sqliteConn == null || sqliteConn.State != System.Data.ConnectionState.Open)
            {
                MessageBox.Show("Database connection is not initialized or open.");
                return;
            }

            if (serialPort == null)
            {
                MessageBox.Show("Serial port is not initialized.");
                return;
            }

            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open(); // Attempt to open the serial port
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening serial port: {ex.Message}");
                    return;
                }
            }

            try
            {
                String commandStr = command.ToString();
                serialPort.Write(commandStr);
                string mode = modeComboBox.SelectedItem?.ToString() ?? "Nomal";

                string insertQuery = "INSERT INTO traffic_commands (command, mode, timestamp) VALUES (@command, @mode, @timestamp)";

                using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, sqliteConn))
                {
                    cmd.Parameters.AddWithValue("@command", commandStr);
                    cmd.Parameters.AddWithValue("@mode", mode);

                    // Lấy thời gian hiện tại theo múi giờ Việt Nam
                    DateTime vietnamTime = DateTime.UtcNow.AddHours(7);
                    cmd.Parameters.AddWithValue("@timestamp", vietnamTime.ToString("yyyy-MM-dd HH:mm:ss"));

                    cmd.ExecuteNonQuery();
                }

                textBox1.Text += $"Sent command to Arduino: {commandStr} (Mode: {mode})\r\n";


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending command to Arduino: {ex.Message}");
            }
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isNightMode && !isManualMode)
            {
                timeLeft--;

                if (timeLeft <= 0)
                {
                    SwitchLights();
                    SetTrafficTimes();
                }

                UpdateTrafficLights();
            }
            else if (isNightMode)
            {
                if (northSouthLight.BackColor == Color.Yellow)
                {
                    northSouthLight.BackColor = SystemColors.Control;
                    eastWestLight.BackColor = Color.Yellow;
                }
                else
                {
                    eastWestLight.BackColor = SystemColors.Control;
                    northSouthLight.BackColor = Color.Yellow;
                }
            }
        }

        private void SwitchLights()
        {
            isNorthSouthGreen = !isNorthSouthGreen;

            northSouthLight.BackColor = isNorthSouthGreen ? Color.Green : Color.Red;
            eastWestLight.BackColor = isNorthSouthGreen ? Color.Red : Color.Green;
        }

        private void SetTrafficTimes()
        {
            switch (modeComboBox.SelectedItem?.ToString())
            {
                case "Peak Hour":
                    timeLeft = 75;
                    isNightMode = false;
                    isManualMode = false;
                    timer.Start();
                    switchLightButton.Enabled = false;
                    SendToArduino('4');
                    break;
                case "Night":
                    timeLeft = 0;
                    isNightMode = true;
                    isManualMode = false;
                    timer.Start();
                    switchLightButton.Enabled = false;
                    SendToArduino('2');
                    break;
                case "Manual":
                    isNightMode = false;
                    isManualMode = true;
                    timer.Stop();
                    switchLightButton.Enabled = true;
                    manualToggleState = false;
                    SendToArduino('5');
                    break;
                default:
                    timeLeft = 60;
                    isNightMode = false;
                    isManualMode = false;
                    timer.Start();
                    switchLightButton.Enabled = false;
                    SendToArduino('3');
                    break;
            }

            UpdateTrafficLights();
        }

        private void UpdateTrafficLights()
        {
            northSouthTimeLabel.Text = (isNightMode || isManualMode) ? "" : $"{timeTrafficLight1} sec";
            eastWestTimeLabel.Text = (isNightMode || isManualMode) ? "" : $"{timeTrafficLight2} sec";

            if (!isNightMode && !isManualMode)
            {
                northSouthLight.BackColor = isNorthSouthGreen ? Color.Green : Color.Red;
                eastWestLight.BackColor = isNorthSouthGreen ? Color.Red : Color.Green;
            }
        }

        private void modeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTrafficTimes();
        }

        private void switchLightButton_Click(object sender, EventArgs e)
        {
            if (isManualMode)
            {
                manualToggleState = !manualToggleState;
                char command = manualToggleState ? '5' : '5';
                SendToArduino(command);

                SwitchLights();
                UpdateTrafficLights();
            }
        }



        // Implement the label1_Click event handler here
        private void label1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Label 1 was clicked.");
        }

        private void InitializeDatabase()
        {
            string connectionString = "Data Source=traffic_light.db;Version=3;";
            sqliteConn = new SQLiteConnection(connectionString);

            try
            {
                sqliteConn.Open();
                if (sqliteConn.State == System.Data.ConnectionState.Open)
                {
                    MessageBox.Show("Database connection successfully opened.");

                    string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS traffic_commands (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    command TEXT NOT NULL,
                    mode TEXT NOT NULL,
                    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                );";

                    using (SQLiteCommand cmd = new SQLiteCommand(createTableQuery, sqliteConn))
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Table 'traffic_commands' created or already exists.");
                    }
                }
                else
                {
                    MessageBox.Show("Failed to open database connection.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing database: {ex.Message}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (sqliteConn != null && sqliteConn.State == System.Data.ConnectionState.Open)
            {
                sqliteConn.Close();
            }

            base.OnFormClosing(e);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
