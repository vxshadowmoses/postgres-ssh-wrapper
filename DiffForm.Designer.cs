namespace DB_NAVIGATOR
{
    partial class DiffForm
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
            this.components = new System.ComponentModel.Container();
            this.lstBox1 = new System.Windows.Forms.ListBox();
            this.lstBox2 = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ddlListConnection1 = new System.Windows.Forms.ComboBox();
            this.ddlListConnection2 = new System.Windows.Forms.ComboBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnCompare = new System.Windows.Forms.Button();
            this.resultDiff = new System.Windows.Forms.TreeView();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // lstBox1
            // 
            this.lstBox1.FormattingEnabled = true;
            this.lstBox1.ItemHeight = 20;
            this.lstBox1.Location = new System.Drawing.Point(12, 46);
            this.lstBox1.Name = "lstBox1";
            this.lstBox1.Size = new System.Drawing.Size(203, 324);
            this.lstBox1.TabIndex = 0;
            // 
            // lstBox2
            // 
            this.lstBox2.FormattingEnabled = true;
            this.lstBox2.ItemHeight = 20;
            this.lstBox2.Location = new System.Drawing.Point(694, 46);
            this.lstBox2.Name = "lstBox2";
            this.lstBox2.Size = new System.Drawing.Size(203, 324);
            this.lstBox2.TabIndex = 1;
            this.lstBox2.SelectedIndexChanged += new System.EventHandler(this.lstBox2_SelectedIndexChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // ddlListConnection1
            // 
            this.ddlListConnection1.FormattingEnabled = true;
            this.ddlListConnection1.Location = new System.Drawing.Point(12, 12);
            this.ddlListConnection1.Name = "ddlListConnection1";
            this.ddlListConnection1.Size = new System.Drawing.Size(203, 28);
            this.ddlListConnection1.TabIndex = 3;
            // 
            // ddlListConnection2
            // 
            this.ddlListConnection2.FormattingEnabled = true;
            this.ddlListConnection2.Location = new System.Drawing.Point(695, 12);
            this.ddlListConnection2.Name = "ddlListConnection2";
            this.ddlListConnection2.Size = new System.Drawing.Size(203, 28);
            this.ddlListConnection2.TabIndex = 4;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(12, 385);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(101, 36);
            this.btnLoad.TabIndex = 6;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnDdlCompare_Click);
            // 
            // btnCompare
            // 
            this.btnCompare.Location = new System.Drawing.Point(114, 385);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(101, 36);
            this.btnCompare.TabIndex = 7;
            this.btnCompare.Text = "Compare";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // resultDiff
            // 
            this.resultDiff.Location = new System.Drawing.Point(221, 46);
            this.resultDiff.Name = "resultDiff";
            this.resultDiff.Size = new System.Drawing.Size(467, 324);
            this.resultDiff.TabIndex = 8;
            this.resultDiff.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.resultDiff_AfterSelect);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 427);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(885, 29);
            this.progressBar.TabIndex = 9;
            // 
            // DiffForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(910, 476);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.resultDiff);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.ddlListConnection2);
            this.Controls.Add(this.ddlListConnection1);
            this.Controls.Add(this.lstBox2);
            this.Controls.Add(this.lstBox1);
            this.Name = "DiffForm";
            this.Text = "DB Diff";
            this.Load += new System.EventHandler(this.DiffForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstBox1;
        private System.Windows.Forms.ListBox lstBox2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ComboBox ddlListConnection1;
        private System.Windows.Forms.ComboBox ddlListConnection2;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.TreeView resultDiff;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}