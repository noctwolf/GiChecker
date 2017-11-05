namespace GiChecker
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.rtbIP = new System.Windows.Forms.RichTextBox();
            this.buttonCheck = new System.Windows.Forms.Button();
            this.buttonHttp = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button扫描全球 = new System.Windows.Forms.Button();
            this.labelCount = new System.Windows.Forms.Label();
            this.button更新 = new System.Windows.Forms.Button();
            this.buttonMMF = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.IP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.延迟 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.类型 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.颁发者 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.颁发给 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.谷歌 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.归属地 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbIP
            // 
            this.rtbIP.Location = new System.Drawing.Point(0, 30);
            this.rtbIP.Name = "rtbIP";
            this.rtbIP.Size = new System.Drawing.Size(100, 381);
            this.rtbIP.TabIndex = 0;
            this.rtbIP.Text = "119.28.84.182\n188.43.64.60\n188.43.64.62\n188.43.64.61\n179.5.71.184\n179.5.71.151\n17" +
    "9.5.71.158\n179.5.71.140\n179.5.71.144\n179.5.71.156\n179.5.71.172\n179.5.71.142\n179." +
    "5.71.185";
            // 
            // buttonCheck
            // 
            this.buttonCheck.Location = new System.Drawing.Point(9, 237);
            this.buttonCheck.Name = "buttonCheck";
            this.buttonCheck.Size = new System.Drawing.Size(75, 23);
            this.buttonCheck.TabIndex = 1;
            this.buttonCheck.Text = "Check";
            this.buttonCheck.UseVisualStyleBackColor = true;
            this.buttonCheck.Click += new System.EventHandler(this.buttonCheck_Click);
            // 
            // buttonHttp
            // 
            this.buttonHttp.Location = new System.Drawing.Point(9, 266);
            this.buttonHttp.Name = "buttonHttp";
            this.buttonHttp.Size = new System.Drawing.Size(75, 23);
            this.buttonHttp.TabIndex = 2;
            this.buttonHttp.Text = "HttpClient";
            this.buttonHttp.UseVisualStyleBackColor = true;
            this.buttonHttp.Click += new System.EventHandler(this.buttonHttp_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IP,
            this.延迟,
            this.类型,
            this.颁发者,
            this.颁发给,
            this.谷歌,
            this.归属地});
            this.dataGridView1.DataSource = this.bindingSource1;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 30);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 50;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(784, 381);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView1_RowPostPaint);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.button扫描全球);
            this.flowLayoutPanel1.Controls.Add(this.labelCount);
            this.flowLayoutPanel1.Controls.Add(this.button更新);
            this.flowLayoutPanel1.Controls.Add(this.buttonMMF);
            this.flowLayoutPanel1.Controls.Add(this.button1);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(784, 30);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // button扫描全球
            // 
            this.button扫描全球.Location = new System.Drawing.Point(3, 3);
            this.button扫描全球.Name = "button扫描全球";
            this.button扫描全球.Size = new System.Drawing.Size(75, 23);
            this.button扫描全球.TabIndex = 0;
            this.button扫描全球.Text = "扫描全球";
            this.button扫描全球.UseVisualStyleBackColor = true;
            this.button扫描全球.Click += new System.EventHandler(this.button扫描全球_Click);
            // 
            // labelCount
            // 
            this.labelCount.AutoSize = true;
            this.labelCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCount.Location = new System.Drawing.Point(84, 0);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(29, 29);
            this.labelCount.TabIndex = 2;
            this.labelCount.Text = "进度";
            this.labelCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button更新
            // 
            this.button更新.Location = new System.Drawing.Point(119, 3);
            this.button更新.Name = "button更新";
            this.button更新.Size = new System.Drawing.Size(75, 23);
            this.button更新.TabIndex = 3;
            this.button更新.Text = "更新";
            this.button更新.UseVisualStyleBackColor = true;
            this.button更新.Click += new System.EventHandler(this.button更新_Click);
            // 
            // buttonMMF
            // 
            this.buttonMMF.Location = new System.Drawing.Point(200, 3);
            this.buttonMMF.Name = "buttonMMF";
            this.buttonMMF.Size = new System.Drawing.Size(75, 23);
            this.buttonMMF.TabIndex = 1;
            this.buttonMMF.Text = "MMF IP";
            this.buttonMMF.UseVisualStyleBackColor = true;
            this.buttonMMF.Click += new System.EventHandler(this.buttonMMF_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(281, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // IP
            // 
            this.IP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.IP.DataPropertyName = "IP";
            this.IP.Frozen = true;
            this.IP.HeaderText = "IP";
            this.IP.MinimumWidth = 100;
            this.IP.Name = "IP";
            this.IP.ReadOnly = true;
            // 
            // 延迟
            // 
            this.延迟.DataPropertyName = "RoundtripTime";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.延迟.DefaultCellStyle = dataGridViewCellStyle1;
            this.延迟.FillWeight = 50F;
            this.延迟.HeaderText = "延迟";
            this.延迟.MinimumWidth = 50;
            this.延迟.Name = "延迟";
            this.延迟.ReadOnly = true;
            // 
            // 类型
            // 
            this.类型.DataPropertyName = "Server";
            this.类型.FillWeight = 50F;
            this.类型.HeaderText = "类型";
            this.类型.MinimumWidth = 50;
            this.类型.Name = "类型";
            this.类型.ReadOnly = true;
            // 
            // 颁发者
            // 
            this.颁发者.DataPropertyName = "Issuer";
            this.颁发者.FillWeight = 280F;
            this.颁发者.HeaderText = "颁发者";
            this.颁发者.MinimumWidth = 280;
            this.颁发者.Name = "颁发者";
            this.颁发者.ReadOnly = true;
            this.颁发者.Visible = false;
            // 
            // 颁发给
            // 
            this.颁发给.DataPropertyName = "Subject";
            this.颁发给.FillWeight = 280F;
            this.颁发给.HeaderText = "证书";
            this.颁发给.MinimumWidth = 280;
            this.颁发给.Name = "颁发给";
            this.颁发给.ReadOnly = true;
            // 
            // 谷歌
            // 
            this.谷歌.DataPropertyName = "IsGoogle";
            this.谷歌.FillWeight = 55F;
            this.谷歌.HeaderText = "谷歌";
            this.谷歌.MinimumWidth = 55;
            this.谷歌.Name = "谷歌";
            this.谷歌.ReadOnly = true;
            this.谷歌.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.谷歌.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.谷歌.Visible = false;
            // 
            // 归属地
            // 
            this.归属地.DataPropertyName = "Location";
            this.归属地.FillWeight = 50F;
            this.归属地.HeaderText = "归属地";
            this.归属地.MinimumWidth = 50;
            this.归属地.Name = "归属地";
            this.归属地.ReadOnly = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 411);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.rtbIP);
            this.Controls.Add(this.buttonHttp);
            this.Controls.Add(this.buttonCheck);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GiChecker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbIP;
        private System.Windows.Forms.Button buttonCheck;
        private System.Windows.Forms.Button buttonHttp;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button button扫描全球;
        private System.Windows.Forms.Button buttonMMF;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.Button button更新;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridViewTextBoxColumn IP;
        private System.Windows.Forms.DataGridViewTextBoxColumn 延迟;
        private System.Windows.Forms.DataGridViewTextBoxColumn 类型;
        private System.Windows.Forms.DataGridViewTextBoxColumn 颁发者;
        private System.Windows.Forms.DataGridViewTextBoxColumn 颁发给;
        private System.Windows.Forms.DataGridViewCheckBoxColumn 谷歌;
        private System.Windows.Forms.DataGridViewTextBoxColumn 归属地;
    }
}

