namespace DemoVSS
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
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.cbFlow = new System.Windows.Forms.ComboBox();
            this.lvLog = new System.Windows.Forms.ListView();
            this.TimeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.InfoHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbLog = new System.Windows.Forms.GroupBox();
            this.gbProcedure = new System.Windows.Forms.GroupBox();
            this.gbSolution = new System.Windows.Forms.GroupBox();
            this.gbResult = new System.Windows.Forms.GroupBox();
            this.listBoxResult = new System.Windows.Forms.ListBox();
            this.lbresult = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gbLog.SuspendLayout();
            this.gbProcedure.SuspendLayout();
            this.gbSolution.SuspendLayout();
            this.gbResult.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(6, 19);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(120, 40);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "Select SolutionPath";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(142, 19);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(120, 40);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load Solution";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(148, 19);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(120, 40);
            this.btnRun.TabIndex = 2;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // cbFlow
            // 
            this.cbFlow.FormattingEnabled = true;
            this.cbFlow.Location = new System.Drawing.Point(6, 28);
            this.cbFlow.Name = "cbFlow";
            this.cbFlow.Size = new System.Drawing.Size(120, 21);
            this.cbFlow.TabIndex = 3;
            this.cbFlow.SelectedIndexChanged += new System.EventHandler(this.comboBoxAllProcedure_SelectedIndexChanged);
            // 
            // lvLog
            // 
            this.lvLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TimeHeader,
            this.InfoHeader});
            this.lvLog.HideSelection = false;
            this.lvLog.Location = new System.Drawing.Point(9, 19);
            this.lvLog.Name = "lvLog";
            this.lvLog.Size = new System.Drawing.Size(259, 450);
            this.lvLog.TabIndex = 4;
            this.lvLog.UseCompatibleStateImageBehavior = false;
            this.lvLog.View = System.Windows.Forms.View.Details;
            // 
            // TimeHeader
            // 
            this.TimeHeader.Text = "Time";
            this.TimeHeader.Width = 110;
            // 
            // InfoHeader
            // 
            this.InfoHeader.Text = "Info";
            this.InfoHeader.Width = 264;
            // 
            // gbLog
            // 
            this.gbLog.Controls.Add(this.lvLog);
            this.gbLog.Location = new System.Drawing.Point(12, 186);
            this.gbLog.Name = "gbLog";
            this.gbLog.Size = new System.Drawing.Size(274, 488);
            this.gbLog.TabIndex = 6;
            this.gbLog.TabStop = false;
            this.gbLog.Text = "Log";
            // 
            // gbProcedure
            // 
            this.gbProcedure.Controls.Add(this.cbFlow);
            this.gbProcedure.Controls.Add(this.btnRun);
            this.gbProcedure.Location = new System.Drawing.Point(12, 93);
            this.gbProcedure.Name = "gbProcedure";
            this.gbProcedure.Size = new System.Drawing.Size(274, 73);
            this.gbProcedure.TabIndex = 7;
            this.gbProcedure.TabStop = false;
            this.gbProcedure.Text = "Procedure Operation";
            // 
            // gbSolution
            // 
            this.gbSolution.Controls.Add(this.btnSelect);
            this.gbSolution.Controls.Add(this.btnLoad);
            this.gbSolution.Location = new System.Drawing.Point(12, 12);
            this.gbSolution.Name = "gbSolution";
            this.gbSolution.Size = new System.Drawing.Size(268, 73);
            this.gbSolution.TabIndex = 8;
            this.gbSolution.TabStop = false;
            this.gbSolution.Text = "Solution Operation";
            // 
            // gbResult
            // 
            this.gbResult.Controls.Add(this.listBoxResult);
            this.gbResult.Controls.Add(this.lbresult);
            this.gbResult.Location = new System.Drawing.Point(292, 550);
            this.gbResult.Name = "gbResult";
            this.gbResult.Size = new System.Drawing.Size(929, 124);
            this.gbResult.TabIndex = 9;
            this.gbResult.TabStop = false;
            this.gbResult.Text = "result";
            // 
            // listBoxResult
            // 
            this.listBoxResult.FormattingEnabled = true;
            this.listBoxResult.Location = new System.Drawing.Point(6, 23);
            this.listBoxResult.Name = "listBoxResult";
            this.listBoxResult.Size = new System.Drawing.Size(729, 95);
            this.listBoxResult.TabIndex = 2;
            // 
            // lbresult
            // 
            this.lbresult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbresult.AutoSize = true;
            this.lbresult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lbresult.Font = new System.Drawing.Font("SimSun", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbresult.ForeColor = System.Drawing.Color.White;
            this.lbresult.Location = new System.Drawing.Point(782, 41);
            this.lbresult.Name = "lbresult";
            this.lbresult.Size = new System.Drawing.Size(91, 64);
            this.lbresult.TabIndex = 1;
            this.lbresult.Text = "OK";
            this.lbresult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(298, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(923, 532);
            this.panel1.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1233, 686);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gbResult);
            this.Controls.Add(this.gbSolution);
            this.Controls.Add(this.gbProcedure);
            this.Controls.Add(this.gbLog);
            this.Name = "Form1";
            this.Text = "Demo";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gbLog.ResumeLayout(false);
            this.gbProcedure.ResumeLayout(false);
            this.gbSolution.ResumeLayout(false);
            this.gbResult.ResumeLayout(false);
            this.gbResult.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.ComboBox cbFlow;
        private System.Windows.Forms.ListView lvLog;
        private System.Windows.Forms.ColumnHeader TimeHeader;
        private System.Windows.Forms.ColumnHeader InfoHeader;
        private System.Windows.Forms.GroupBox gbLog;
        private System.Windows.Forms.GroupBox gbProcedure;
        private System.Windows.Forms.GroupBox gbSolution;
        private System.Windows.Forms.GroupBox gbResult;
        private System.Windows.Forms.Label lbresult;
        private System.Windows.Forms.ListBox listBoxResult;
        private System.Windows.Forms.Panel panel1;
    }

}

