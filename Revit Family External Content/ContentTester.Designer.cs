namespace RevitContentWatcher
{
    partial class ContentTester
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonFlatFile = new System.Windows.Forms.Button();
            this.buttonGetFamilies = new System.Windows.Forms.Button();
            this.buttonSaveXMLs = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(800, 371);
            this.dataGridView1.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 389);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(800, 124);
            this.textBox1.TabIndex = 1;
            // 
            // buttonFlatFile
            // 
            this.buttonFlatFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonFlatFile.Location = new System.Drawing.Point(120, 517);
            this.buttonFlatFile.Name = "buttonFlatFile";
            this.buttonFlatFile.Size = new System.Drawing.Size(99, 32);
            this.buttonFlatFile.TabIndex = 2;
            this.buttonFlatFile.Text = "Save a Flat File";
            this.buttonFlatFile.UseVisualStyleBackColor = true;
            this.buttonFlatFile.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonGetFamilies
            // 
            this.buttonGetFamilies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonGetFamilies.Location = new System.Drawing.Point(12, 517);
            this.buttonGetFamilies.Name = "buttonGetFamilies";
            this.buttonGetFamilies.Size = new System.Drawing.Size(92, 32);
            this.buttonGetFamilies.TabIndex = 3;
            this.buttonGetFamilies.Text = "Get families";
            this.buttonGetFamilies.UseVisualStyleBackColor = true;
            this.buttonGetFamilies.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonSaveXMLs
            // 
            this.buttonSaveXMLs.Location = new System.Drawing.Point(235, 517);
            this.buttonSaveXMLs.Name = "buttonSaveXMLs";
            this.buttonSaveXMLs.Size = new System.Drawing.Size(99, 32);
            this.buttonSaveXMLs.TabIndex = 4;
            this.buttonSaveXMLs.Text = "Save XML Files";
            this.buttonSaveXMLs.UseVisualStyleBackColor = true;
            this.buttonSaveXMLs.Click += new System.EventHandler(this.buttonSaveXMLs_Click);
            // 
            // ContentTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 561);
            this.Controls.Add(this.buttonSaveXMLs);
            this.Controls.Add(this.buttonGetFamilies);
            this.Controls.Add(this.buttonFlatFile);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "ContentTester";
            this.Text = "Network Revit Content";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonFlatFile;
        private System.Windows.Forms.Button buttonGetFamilies;
        private System.Windows.Forms.Button buttonSaveXMLs;
    }
}