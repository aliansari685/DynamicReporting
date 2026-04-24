namespace SignalRClientTest.WinForm
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnTest = new Button();
            richTextBoxLog = new RichTextBox();
            btnJoinGroup = new Button();
            btnConnect = new Button();
            txtBox_reportGuid = new TextBox();
            txtBox_Port = new TextBox();
            label1 = new Label();
            btnDisconnect = new Button();
            btnClearRichTextBox = new Button();
            SuspendLayout();
            // 
            // btnTest
            // 
            btnTest.Location = new Point(236, 12);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(75, 23);
            btnTest.TabIndex = 0;
            btnTest.Text = "Test";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += btnTest_Click;
            // 
            // richTextBoxLog
            // 
            richTextBoxLog.Dock = DockStyle.Bottom;
            richTextBoxLog.Location = new Point(0, 84);
            richTextBoxLog.Name = "richTextBoxLog";
            richTextBoxLog.ReadOnly = true;
            richTextBoxLog.Size = new Size(490, 352);
            richTextBoxLog.TabIndex = 1;
            richTextBoxLog.Text = "";
            // 
            // btnJoinGroup
            // 
            btnJoinGroup.Location = new Point(317, 12);
            btnJoinGroup.Name = "btnJoinGroup";
            btnJoinGroup.Size = new Size(75, 23);
            btnJoinGroup.TabIndex = 4;
            btnJoinGroup.Text = "JoinGroup";
            btnJoinGroup.UseVisualStyleBackColor = true;
            btnJoinGroup.Click += btnJoinGroup_Click;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(120, 12);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 23);
            btnConnect.TabIndex = 5;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // txtBox_reportGuid
            // 
            txtBox_reportGuid.Location = new Point(236, 42);
            txtBox_reportGuid.Name = "txtBox_reportGuid";
            txtBox_reportGuid.Size = new Size(156, 23);
            txtBox_reportGuid.TabIndex = 2;
            // 
            // txtBox_Port
            // 
            txtBox_Port.Location = new Point(17, 41);
            txtBox_Port.Name = "txtBox_Port";
            txtBox_Port.Size = new Size(86, 23);
            txtBox_Port.TabIndex = 6;
            txtBox_Port.Text = "7177";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 16);
            label1.Name = "label1";
            label1.Size = new Size(86, 15);
            label1.TabIndex = 7;
            label1.Text = "Localhost Port:";
            // 
            // btnDisconnect
            // 
            btnDisconnect.Location = new Point(120, 41);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(75, 23);
            btnDisconnect.TabIndex = 8;
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // btnClearRichTextBox
            // 
            btnClearRichTextBox.Location = new Point(427, 95);
            btnClearRichTextBox.Name = "btnClearRichTextBox";
            btnClearRichTextBox.Size = new Size(51, 23);
            btnClearRichTextBox.TabIndex = 9;
            btnClearRichTextBox.Text = "Clear";
            btnClearRichTextBox.UseVisualStyleBackColor = true;
            btnClearRichTextBox.Click += btnClearRichTextBox_Click;
            // 
            // FrmMain
            // 
            AcceptButton = btnConnect;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(490, 436);
            Controls.Add(btnClearRichTextBox);
            Controls.Add(btnDisconnect);
            Controls.Add(label1);
            Controls.Add(txtBox_Port);
            Controls.Add(btnConnect);
            Controls.Add(btnJoinGroup);
            Controls.Add(txtBox_reportGuid);
            Controls.Add(richTextBoxLog);
            Controls.Add(btnTest);
            Name = "FrmMain";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnTest;
        private RichTextBox richTextBoxLog;
        private Button btnJoinGroup;
        private Button btnConnect;
        private TextBox txtBox_reportGuid;
        private TextBox txtBox_Port;
        private Label label1;
        private Button btnDisconnect;
        private Button btnClearRichTextBox;
    }
}
