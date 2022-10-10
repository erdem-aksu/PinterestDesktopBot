namespace PinterestDesktopBot
{
    partial class ProxyForm2
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
            this.button1 = new System.Windows.Forms.Button();
            this.emailVerify = new System.Windows.Forms.CheckBox();
            this.accountCountBox = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panoBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.countryComboBox = new System.Windows.Forms.ComboBox();
            this.createApi = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.simultaneousBox = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.proxyListBox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize) (this.accountCountBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.simultaneousBox)).BeginInit();
            this.groupBox3.SuspendLayout();
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
            this.emailVerify.Location = new System.Drawing.Point(6, 108);
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
            this.groupBox1.Location = new System.Drawing.Point(12, 334);
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
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.simultaneousBox);
            this.groupBox2.Controls.Add(this.panoBox);
            this.groupBox2.Controls.Add(this.label4);
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
            this.panoBox.Location = new System.Drawing.Point(128, 168);
            this.panoBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panoBox.Name = "panoBox";
            this.panoBox.Size = new System.Drawing.Size(121, 27);
            this.panoBox.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 22);
            this.label4.TabIndex = 12;
            this.label4.Text = "Pano";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 22);
            this.label2.TabIndex = 9;
            this.label2.Text = "Ülke";
            // 
            // countryComboBox
            // 
            this.countryComboBox.FormattingEnabled = true;
            this.countryComboBox.Location = new System.Drawing.Point(128, 136);
            this.countryComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.countryComboBox.Name = "countryComboBox";
            this.countryComboBox.Size = new System.Drawing.Size(121, 28);
            this.countryComboBox.TabIndex = 8;
            // 
            // createApi
            // 
            this.createApi.Location = new System.Drawing.Point(6, 85);
            this.createApi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.createApi.Name = "createApi";
            this.createApi.Size = new System.Drawing.Size(244, 24);
            this.createApi.TabIndex = 7;
            this.createApi.Text = "Api Oluştur";
            this.createApi.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 22);
            this.label3.TabIndex = 15;
            this.label3.Text = "Eşzamanlı İşlem";
            // 
            // simultaneousBox
            // 
            this.simultaneousBox.Location = new System.Drawing.Point(130, 54);
            this.simultaneousBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.simultaneousBox.Maximum = new decimal(new int[] {10000, 0, 0, 0});
            this.simultaneousBox.Name = "simultaneousBox";
            this.simultaneousBox.Size = new System.Drawing.Size(120, 27);
            this.simultaneousBox.TabIndex = 14;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.proxyListBox);
            this.groupBox3.Location = new System.Drawing.Point(286, 32);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Size = new System.Drawing.Size(229, 285);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Proxy List";
            // 
            // proxyListBox
            // 
            this.proxyListBox.Location = new System.Drawing.Point(6, 25);
            this.proxyListBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.proxyListBox.Name = "proxyListBox";
            this.proxyListBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.proxyListBox.Size = new System.Drawing.Size(217, 256);
            this.proxyListBox.TabIndex = 0;
            this.proxyListBox.Text = "";
            // 
            // ProxyForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 636);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ProxyForm2";
            this.Text = "Pinterest Desktop Bot";
            ((System.ComponentModel.ISupportInitialize) (this.accountCountBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.simultaneousBox)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox createApi;
        private System.Windows.Forms.CheckBox emailVerify;
        private System.Windows.Forms.NumericUpDown accountCountBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox countryComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox panoBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown simultaneousBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox proxyListBox;
        public System.Windows.Forms.RichTextBox richTextBox1;
    }
}