namespace PictureHandler {
  partial class Form1 {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
			this.tbRawInputFolder = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbRenamedFolder = new System.Windows.Forms.TextBox();
			this.btnProcessRawInput = new System.Windows.Forms.Button();
			this.tbResizedFolder = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnProcessRenamed = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tbRawInputFolder
			// 
			this.tbRawInputFolder.Location = new System.Drawing.Point(106, 44);
			this.tbRawInputFolder.Name = "tbRawInputFolder";
			this.tbRawInputFolder.Size = new System.Drawing.Size(276, 20);
			this.tbRawInputFolder.TabIndex = 0;
			this.tbRawInputFolder.Text = "D:\\temp\\to be processed";
			this.tbRawInputFolder.TextChanged += new System.EventHandler(this.tbToConvertFolder_TextChanged);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 47);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Raw Input folder";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 73);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Renamed folder";
			// 
			// tbRenamedFolder
			// 
			this.tbRenamedFolder.Location = new System.Drawing.Point(106, 70);
			this.tbRenamedFolder.Name = "tbRenamedFolder";
			this.tbRenamedFolder.Size = new System.Drawing.Size(276, 20);
			this.tbRenamedFolder.TabIndex = 2;
			this.tbRenamedFolder.Text = "D:\\temp\\renamed";
			// 
			// btnProcessRawInput
			// 
			this.btnProcessRawInput.Location = new System.Drawing.Point(390, 56);
			this.btnProcessRawInput.Name = "btnProcessRawInput";
			this.btnProcessRawInput.Size = new System.Drawing.Size(118, 23);
			this.btnProcessRawInput.TabIndex = 4;
			this.btnProcessRawInput.Text = "Process Raw Input";
			this.btnProcessRawInput.UseVisualStyleBackColor = true;
			this.btnProcessRawInput.Click += new System.EventHandler(this.btnProcessRawInput_Click);
			// 
			// tbResizedFolder
			// 
			this.tbResizedFolder.Location = new System.Drawing.Point(106, 96);
			this.tbResizedFolder.Name = "tbResizedFolder";
			this.tbResizedFolder.Size = new System.Drawing.Size(276, 20);
			this.tbResizedFolder.TabIndex = 2;
			this.tbResizedFolder.Text = "D:\\temp\\resized";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(14, 99);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(74, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Resized folder";
			// 
			// btnProcessRenamed
			// 
			this.btnProcessRenamed.Location = new System.Drawing.Point(390, 82);
			this.btnProcessRenamed.Name = "btnProcessRenamed";
			this.btnProcessRenamed.Size = new System.Drawing.Size(118, 23);
			this.btnProcessRenamed.TabIndex = 4;
			this.btnProcessRenamed.Text = "Process Renamed";
			this.btnProcessRenamed.UseVisualStyleBackColor = true;
			this.btnProcessRenamed.Click += new System.EventHandler(this.btnProcessRenamed_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(545, 476);
			this.Controls.Add(this.btnProcessRenamed);
			this.Controls.Add(this.btnProcessRawInput);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbResizedFolder);
			this.Controls.Add(this.tbRenamedFolder);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbRawInputFolder);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox tbRawInputFolder;
    private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbRenamedFolder;
		private System.Windows.Forms.Button btnProcessRawInput;
		private System.Windows.Forms.TextBox tbResizedFolder;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnProcessRenamed;
  }
}

