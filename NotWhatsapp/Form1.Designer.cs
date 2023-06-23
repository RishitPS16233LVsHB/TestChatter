namespace NotWhatsapp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtYourPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnListen = new System.Windows.Forms.Button();
            this.lsClients = new System.Windows.Forms.ListBox();
            this.lsMessages = new System.Windows.Forms.ListBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnConnectToServer = new System.Windows.Forms.Button();
            this.txtClientIPAndPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServerIpAndPort = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtYourPort
            // 
            this.txtYourPort.Location = new System.Drawing.Point(72, 19);
            this.txtYourPort.Name = "txtYourPort";
            this.txtYourPort.Size = new System.Drawing.Size(115, 20);
            this.txtYourPort.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "your port";
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(193, 17);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(75, 23);
            this.btnListen.TabIndex = 7;
            this.btnListen.Text = "Start App Listening";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // lsClients
            // 
            this.lsClients.FormattingEnabled = true;
            this.lsClients.Location = new System.Drawing.Point(16, 45);
            this.lsClients.Name = "lsClients";
            this.lsClients.Size = new System.Drawing.Size(297, 537);
            this.lsClients.TabIndex = 8;
            // 
            // lsMessages
            // 
            this.lsMessages.FormattingEnabled = true;
            this.lsMessages.Location = new System.Drawing.Point(319, 45);
            this.lsMessages.Name = "lsMessages";
            this.lsMessages.Size = new System.Drawing.Size(1087, 433);
            this.lsMessages.TabIndex = 9;
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(319, 484);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(1087, 47);
            this.txtMessage.TabIndex = 10;
            // 
            // btnSend
            // 
            this.btnSend.BackColor = System.Drawing.Color.Lime;
            this.btnSend.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSend.Location = new System.Drawing.Point(319, 537);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(119, 45);
            this.btnSend.TabIndex = 11;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnConnectToServer
            // 
            this.btnConnectToServer.Location = new System.Drawing.Point(688, 18);
            this.btnConnectToServer.Name = "btnConnectToServer";
            this.btnConnectToServer.Size = new System.Drawing.Size(172, 23);
            this.btnConnectToServer.TabIndex = 12;
            this.btnConnectToServer.Text = "Connect To this Server";
            this.btnConnectToServer.UseVisualStyleBackColor = true;
            this.btnConnectToServer.Click += new System.EventHandler(this.btnConnectToServer_Click);
            // 
            // txtClientIPAndPort
            // 
            this.txtClientIPAndPort.Location = new System.Drawing.Point(610, 547);
            this.txtClientIPAndPort.Name = "txtClientIPAndPort";
            this.txtClientIPAndPort.Size = new System.Drawing.Size(125, 20);
            this.txtClientIPAndPort.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(467, 550);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Friends <IPAddress : port> :- ";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(741, 537);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(119, 39);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Connect To A Friend";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(274, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Server <IPAddress : Port number> :- ";
            // 
            // txtServerIpAndPort
            // 
            this.txtServerIpAndPort.Location = new System.Drawing.Point(459, 21);
            this.txtServerIpAndPort.Name = "txtServerIpAndPort";
            this.txtServerIpAndPort.Size = new System.Drawing.Size(223, 20);
            this.txtServerIpAndPort.TabIndex = 13;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1418, 594);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtServerIpAndPort);
            this.Controls.Add(this.btnConnectToServer);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.lsMessages);
            this.Controls.Add(this.lsClients);
            this.Controls.Add(this.btnListen);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtClientIPAndPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtYourPort);
            this.Name = "Form1";
            this.Text = "Not Whatsapp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtYourPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.ListBox lsClients;
        private System.Windows.Forms.ListBox lsMessages;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnConnectToServer;
        private System.Windows.Forms.TextBox txtClientIPAndPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServerIpAndPort;
    }
}

