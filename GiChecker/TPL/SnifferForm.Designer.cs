namespace GiChecker.TPL
{
    partial class SnifferForm
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
            this.nudTimeout = new System.Windows.Forms.NumericUpDown();
            this.nudThread = new System.Windows.Forms.NumericUpDown();
            this.buttonOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThread)).BeginInit();
            this.SuspendLayout();
            // 
            // nudTimeout
            // 
            this.nudTimeout.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudTimeout.Location = new System.Drawing.Point(82, 12);
            this.nudTimeout.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTimeout.Name = "nudTimeout";
            this.nudTimeout.Size = new System.Drawing.Size(120, 21);
            this.nudTimeout.TabIndex = 0;
            this.nudTimeout.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // nudThread
            // 
            this.nudThread.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudThread.Location = new System.Drawing.Point(82, 39);
            this.nudThread.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudThread.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudThread.Name = "nudThread";
            this.nudThread.Size = new System.Drawing.Size(120, 21);
            this.nudThread.TabIndex = 1;
            this.nudThread.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(222, 75);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // SnifferForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 110);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.nudThread);
            this.Controls.Add(this.nudTimeout);
            this.Name = "SnifferForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SnifferParam";
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThread)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudTimeout;
        private System.Windows.Forms.NumericUpDown nudThread;
        private System.Windows.Forms.Button buttonOk;
    }
}