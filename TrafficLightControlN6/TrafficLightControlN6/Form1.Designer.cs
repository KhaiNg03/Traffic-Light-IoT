namespace TrafficLightControlN6
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel northSouthLight;
        private System.Windows.Forms.Panel eastWestLight;
        private System.Windows.Forms.Label northSouthTimeLabel;
        private System.Windows.Forms.Label eastWestTimeLabel;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button switchLightButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.northSouthLight = new System.Windows.Forms.Panel();
            this.eastWestLight = new System.Windows.Forms.Panel();
            this.northSouthTimeLabel = new System.Windows.Forms.Label();
            this.eastWestTimeLabel = new System.Windows.Forms.Label();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.switchLightButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // northSouthLight
            // 
            this.northSouthLight.BackColor = System.Drawing.SystemColors.Control;
            this.northSouthLight.Location = new System.Drawing.Point(50, 50);
            this.northSouthLight.Name = "northSouthLight";
            this.northSouthLight.Size = new System.Drawing.Size(150, 150);
            this.northSouthLight.TabIndex = 0;
            // 
            // eastWestLight
            // 
            this.eastWestLight.BackColor = System.Drawing.SystemColors.Control;
            this.eastWestLight.Location = new System.Drawing.Point(250, 50);
            this.eastWestLight.Name = "eastWestLight";
            this.eastWestLight.Size = new System.Drawing.Size(150, 150);
            this.eastWestLight.TabIndex = 1;
            //this.eastWestLight.Paint += new System.Windows.Forms.PaintEventHandler(this.eastWestLight_Paint);
            // 
            // northSouthTimeLabel
            // 
            this.northSouthTimeLabel.AutoSize = true;
            this.northSouthTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.northSouthTimeLabel.Location = new System.Drawing.Point(80, 225);
            this.northSouthTimeLabel.Name = "northSouthTimeLabel";
            this.northSouthTimeLabel.Size = new System.Drawing.Size(76, 26);
            this.northSouthTimeLabel.TabIndex = 2;
            this.northSouthTimeLabel.Text = "60 sec";
            // 
            // eastWestTimeLabel
            // 
            this.eastWestTimeLabel.AutoSize = true;
            this.eastWestTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.eastWestTimeLabel.Location = new System.Drawing.Point(280, 225);
            this.eastWestTimeLabel.Name = "eastWestTimeLabel";
            this.eastWestTimeLabel.Size = new System.Drawing.Size(76, 26);
            this.eastWestTimeLabel.TabIndex = 3;
            this.eastWestTimeLabel.Text = "60 sec";
            // 
            // modeComboBox
            // 
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Items.AddRange(new object[] {
            "Normal",
            "Peak Hour",
            "Night",
            "Manual"});
            this.modeComboBox.Location = new System.Drawing.Point(240, 300);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(121, 21);
            this.modeComboBox.TabIndex = 4;
            this.modeComboBox.SelectedIndexChanged += new System.EventHandler(this.modeComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(111, 303);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Operating Mode:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // switchLightButton
            // 
            this.switchLightButton.Enabled = false;
            this.switchLightButton.Location = new System.Drawing.Point(159, 352);
            this.switchLightButton.Name = "switchLightButton";
            this.switchLightButton.Size = new System.Drawing.Size(150, 50);
            this.switchLightButton.TabIndex = 6;
            this.switchLightButton.Text = "Switch Light";
            this.switchLightButton.UseVisualStyleBackColor = true;
            this.switchLightButton.Click += new System.EventHandler(this.switchLightButton_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(32, 425);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(450, 125);
            this.textBox1.TabIndex = 8;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(536, 580);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.switchLightButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.modeComboBox);
            this.Controls.Add(this.eastWestTimeLabel);
            this.Controls.Add(this.northSouthTimeLabel);
            this.Controls.Add(this.eastWestLight);
            this.Controls.Add(this.northSouthLight);
            this.Name = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TextBox textBox1;
    }
}
