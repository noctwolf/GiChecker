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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.IP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.延迟 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.类型 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.颁发者 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.颁发给 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.谷歌 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.归属地 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button扫描全球 = new System.Windows.Forms.Button();
            this.labelCount = new System.Windows.Forms.Label();
            this.button更新 = new System.Windows.Forms.Button();
            this.buttonMMF = new System.Windows.Forms.Button();
            this.buttonGoogleIP = new System.Windows.Forms.Button();
            this.buttonGoogleIPHunter = new System.Windows.Forms.Button();
            this.buttonIPv4Location = new System.Windows.Forms.Button();
            this.ucFormAlign1 = new GiChecker.UC.UCFormAlign();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
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
            this.dataGridView1.Location = new System.Drawing.Point(0, 58);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 50;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(784, 353);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView1_RowPostPaint);
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
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.ucFormAlign1);
            this.flowLayoutPanel1.Controls.Add(this.button扫描全球);
            this.flowLayoutPanel1.Controls.Add(this.labelCount);
            this.flowLayoutPanel1.Controls.Add(this.button更新);
            this.flowLayoutPanel1.Controls.Add(this.buttonMMF);
            this.flowLayoutPanel1.Controls.Add(this.buttonGoogleIP);
            this.flowLayoutPanel1.Controls.Add(this.buttonGoogleIPHunter);
            this.flowLayoutPanel1.Controls.Add(this.buttonIPv4Location);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(784, 58);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // button扫描全球
            // 
            this.button扫描全球.AutoSize = true;
            this.button扫描全球.Location = new System.Drawing.Point(32, 3);
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
            this.labelCount.Location = new System.Drawing.Point(113, 3);
            this.labelCount.Margin = new System.Windows.Forms.Padding(3);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(29, 23);
            this.labelCount.TabIndex = 2;
            this.labelCount.Text = "进度";
            this.labelCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button更新
            // 
            this.button更新.AutoSize = true;
            this.button更新.Location = new System.Drawing.Point(148, 3);
            this.button更新.Name = "button更新";
            this.button更新.Size = new System.Drawing.Size(75, 23);
            this.button更新.TabIndex = 3;
            this.button更新.Text = "更新";
            this.button更新.UseVisualStyleBackColor = true;
            this.button更新.Click += new System.EventHandler(this.button更新_Click);
            // 
            // buttonMMF
            // 
            this.buttonMMF.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.buttonMMF, true);
            this.buttonMMF.Location = new System.Drawing.Point(229, 3);
            this.buttonMMF.Name = "buttonMMF";
            this.buttonMMF.Size = new System.Drawing.Size(75, 23);
            this.buttonMMF.TabIndex = 1;
            this.buttonMMF.Text = "MMF IP";
            this.buttonMMF.UseVisualStyleBackColor = true;
            this.buttonMMF.Click += new System.EventHandler(this.buttonMMF_Click);
            // 
            // buttonGoogleIP
            // 
            this.buttonGoogleIP.AutoSize = true;
            this.buttonGoogleIP.Location = new System.Drawing.Point(3, 32);
            this.buttonGoogleIP.Name = "buttonGoogleIP";
            this.buttonGoogleIP.Size = new System.Drawing.Size(99, 23);
            this.buttonGoogleIP.TabIndex = 4;
            this.buttonGoogleIP.Text = "google ip duan";
            this.buttonGoogleIP.UseVisualStyleBackColor = true;
            this.buttonGoogleIP.Click += new System.EventHandler(this.buttonGoogleIP_Click);
            // 
            // buttonGoogleIPHunter
            // 
            this.buttonGoogleIPHunter.AutoSize = true;
            this.buttonGoogleIPHunter.Location = new System.Drawing.Point(108, 32);
            this.buttonGoogleIPHunter.Name = "buttonGoogleIPHunter";
            this.buttonGoogleIPHunter.Size = new System.Drawing.Size(99, 23);
            this.buttonGoogleIPHunter.TabIndex = 5;
            this.buttonGoogleIPHunter.Text = "GoogleIPHunter";
            this.buttonGoogleIPHunter.UseVisualStyleBackColor = true;
            this.buttonGoogleIPHunter.Click += new System.EventHandler(this.buttonGoogleIPHunter_Click);
            // 
            // buttonIPv4Location
            // 
            this.buttonIPv4Location.AutoSize = true;
            this.flowLayoutPanel1.SetFlowBreak(this.buttonIPv4Location, true);
            this.buttonIPv4Location.Location = new System.Drawing.Point(213, 32);
            this.buttonIPv4Location.Name = "buttonIPv4Location";
            this.buttonIPv4Location.Size = new System.Drawing.Size(87, 23);
            this.buttonIPv4Location.TabIndex = 6;
            this.buttonIPv4Location.Text = "IPv4Location";
            this.buttonIPv4Location.UseVisualStyleBackColor = true;
            this.buttonIPv4Location.Click += new System.EventHandler(this.buttonIPv4DB_Click);
            // 
            // ucFormAlign1
            // 
            this.ucFormAlign1.Location = new System.Drawing.Point(3, 3);
            this.ucFormAlign1.Name = "ucFormAlign1";
            this.ucFormAlign1.Size = new System.Drawing.Size(23, 23);
            this.ucFormAlign1.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 411);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GiChecker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button button扫描全球;
        private System.Windows.Forms.Button buttonMMF;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.Button button更新;
        private System.Windows.Forms.Button buttonGoogleIP;
        private System.Windows.Forms.DataGridViewTextBoxColumn IP;
        private System.Windows.Forms.DataGridViewTextBoxColumn 延迟;
        private System.Windows.Forms.DataGridViewTextBoxColumn 类型;
        private System.Windows.Forms.DataGridViewTextBoxColumn 颁发者;
        private System.Windows.Forms.DataGridViewTextBoxColumn 颁发给;
        private System.Windows.Forms.DataGridViewCheckBoxColumn 谷歌;
        private System.Windows.Forms.DataGridViewTextBoxColumn 归属地;
        private System.Windows.Forms.Button buttonGoogleIPHunter;
        private System.Windows.Forms.Button buttonIPv4Location;
        private UC.UCFormAlign ucFormAlign1;
    }
}

