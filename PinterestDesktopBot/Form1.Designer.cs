namespace PinterestDesktopBot
{
    partial class Form1
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.emailVerify = new System.Windows.Forms.CheckBox();
            this.accountCountBox = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panoBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.mobileIpBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.countryComboBox = new System.Windows.Forms.ComboBox();
            this.createApi = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.manuelButton1 = new System.Windows.Forms.ToolStripButton();
            this.botButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize) (this.accountCountBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 226);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 41);
            this.button1.TabIndex = 0;
            this.button1.Text = "Başlat";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // emailVerify
            // 
            this.emailVerify.Location = new System.Drawing.Point(6, 82);
            this.emailVerify.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.emailVerify.Name = "emailVerify";
            this.emailVerify.Size = new System.Drawing.Size(244, 24);
            this.emailVerify.TabIndex = 2;
            this.emailVerify.Text = "E-Mail Doğrula";
            this.emailVerify.UseVisualStyleBackColor = true;
            // 
            // accountCountBox
            // 
            this.accountCountBox.Location = new System.Drawing.Point(130, 21);
            this.accountCountBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.accountCountBox.Maximum = new decimal(new int[] {10000, 0, 0, 0});
            this.accountCountBox.Name = "accountCountBox";
            this.accountCountBox.Size = new System.Drawing.Size(120, 27);
            this.accountCountBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 22);
            this.label1.TabIndex = 6;
            this.label1.Text = "Hesap Sayısı";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richTextBox1);
            this.groupBox1.Location = new System.Drawing.Point(285, 32);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(503, 285);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Loglar";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(6, 24);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.richTextBox1.Size = new System.Drawing.Size(491, 256);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panoBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.mobileIpBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.countryComboBox);
            this.groupBox2.Controls.Add(this.createApi);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.emailVerify);
            this.groupBox2.Controls.Add(this.accountCountBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 32);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(267, 285);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ayarlar";
            // 
            // panoBox
            // 
            this.panoBox.Location = new System.Drawing.Point(129, 178);
            this.panoBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panoBox.Name = "panoBox";
            this.panoBox.Size = new System.Drawing.Size(121, 27);
            this.panoBox.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 180);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 22);
            this.label4.TabIndex = 12;
            this.label4.Text = "Pano";
            // 
            // mobileIpBox
            // 
            this.mobileIpBox.Location = new System.Drawing.Point(129, 145);
            this.mobileIpBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.mobileIpBox.Name = "mobileIpBox";
            this.mobileIpBox.Size = new System.Drawing.Size(121, 27);
            this.mobileIpBox.TabIndex = 11;
            this.mobileIpBox.Text = "192.168.43.1";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 22);
            this.label3.TabIndex = 10;
            this.label3.Text = "Mobil IP";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 22);
            this.label2.TabIndex = 9;
            this.label2.Text = "Ülke";
            // 
            // countryComboBox
            // 
            this.countryComboBox.FormattingEnabled = true;
            this.countryComboBox.Location = new System.Drawing.Point(129, 111);
            this.countryComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.countryComboBox.Name = "countryComboBox";
            this.countryComboBox.Size = new System.Drawing.Size(121, 28);
            this.countryComboBox.TabIndex = 8;
            // 
            // createApi
            // 
            this.createApi.Location = new System.Drawing.Point(6, 54);
            this.createApi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.createApi.Name = "createApi";
            this.createApi.Size = new System.Drawing.Size(244, 24);
            this.createApi.TabIndex = 7;
            this.createApi.Text = "Api Oluştur";
            this.createApi.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.manuelButton1, this.botButton1, this.toolStripButton1, this.toolStripButton2, this.toolStripButton3
            });
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 27);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // manuelButton1
            // 
            this.manuelButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.manuelButton1.Image = ((System.Drawing.Image) (resources.GetObject("manuelButton1.Image")));
            this.manuelButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.manuelButton1.Name = "manuelButton1";
            this.manuelButton1.Size = new System.Drawing.Size(113, 24);
            this.manuelButton1.Text = "Manuel Oluştur";
            this.manuelButton1.Click += new System.EventHandler(this.manuelButton1_Click);
            // 
            // botButton1
            // 
            this.botButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.botButton1.Image = ((System.Drawing.Image) (resources.GetObject("botButton1.Image")));
            this.botButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.botButton1.Name = "botButton1";
            this.botButton1.Size = new System.Drawing.Size(99, 24);
            this.botButton1.Text = "Botla Oluştur";
            this.botButton1.Click += new System.EventHandler(this.botButton1_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image) (resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(112, 24);
            this.toolStripButton1.Text = "Proxyle Oluştur";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image) (resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(190, 24);
            this.toolStripButton2.Text = "Proxyle Tarayıcıdan Oluştur";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image) (resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(51, 24);
            this.toolStripButton3.Text = "Repin";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 329);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Pinterest Desktop Bot";
            ((System.ComponentModel.ISupportInitialize) (this.accountCountBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.CheckBox createApi;
        private System.Windows.Forms.CheckBox emailVerify;
        private System.Windows.Forms.NumericUpDown accountCountBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox countryComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox mobileIpBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton manuelButton1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox panoBox;
        private System.Windows.Forms.ToolStripButton botButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
    }
}