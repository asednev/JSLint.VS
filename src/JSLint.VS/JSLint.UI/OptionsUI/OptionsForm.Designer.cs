
namespace JSLint.UI.OptionsUI
{
    partial class OptionsForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
			this.ok = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.enableJSLint = new System.Windows.Forms.CheckBox();
			this.importFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.exportFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.import = new System.Windows.Forms.Button();
			this.export = new System.Windows.Forms.Button();
			this.optionsSource = new System.Windows.Forms.Label();
			this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
			this.optionsUI = new JSLint.UI.OptionsUI.OptionsUI();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// ok
			// 
			this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ok.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ok.Location = new System.Drawing.Point(503, 576);
			this.ok.Name = "ok";
			this.ok.Size = new System.Drawing.Size(75, 32);
			this.ok.TabIndex = 1010;
			this.ok.Text = "&OK";
			this.ok.UseVisualStyleBackColor = true;
			this.ok.Click += new System.EventHandler(this.ok_Click);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cancel.Location = new System.Drawing.Point(594, 576);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(75, 32);
			this.cancel.TabIndex = 1011;
			this.cancel.Text = "&Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 30);
			this.label1.TabIndex = 2;
			this.label1.Text = "Options";
			// 
			// enableJSLint
			// 
			this.enableJSLint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.enableJSLint.AutoSize = true;
			this.enableJSLint.Checked = true;
			this.enableJSLint.CheckState = System.Windows.Forms.CheckState.Checked;
			this.enableJSLint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.enableJSLint.Location = new System.Drawing.Point(608, 21);
			this.enableJSLint.Name = "enableJSLint";
			this.enableJSLint.Size = new System.Drawing.Size(61, 17);
			this.enableJSLint.TabIndex = 0;
			this.enableJSLint.Text = "&Enable";
			this.enableJSLint.UseVisualStyleBackColor = true;
			this.enableJSLint.CheckedChanged += new System.EventHandler(this.enableJSLint_CheckedChanged);
			// 
			// importFileDialog
			// 
			this.importFileDialog.FileName = "JSLintOptions.xml";
			this.importFileDialog.Filter = "XML files|*.xml";
			// 
			// exportFileDialog
			// 
			this.exportFileDialog.DefaultExt = "xml";
			this.exportFileDialog.Filter = "XML files|*.xml";
			// 
			// import
			// 
			this.import.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.import.Font = new System.Drawing.Font("Segoe UI", 9.75F);
			this.import.Location = new System.Drawing.Point(17, 576);
			this.import.Name = "import";
			this.import.Size = new System.Drawing.Size(75, 32);
			this.import.TabIndex = 1012;
			this.import.Text = "Import...";
			this.import.UseVisualStyleBackColor = true;
			this.import.Click += new System.EventHandler(this.import_Click);
			// 
			// export
			// 
			this.export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.export.Font = new System.Drawing.Font("Segoe UI", 9.75F);
			this.export.Location = new System.Drawing.Point(108, 576);
			this.export.Name = "export";
			this.export.Size = new System.Drawing.Size(75, 32);
			this.export.TabIndex = 1013;
			this.export.Text = "Export...";
			this.export.UseVisualStyleBackColor = true;
			this.export.Click += new System.EventHandler(this.export_Click);
			// 
			// optionsSource
			// 
			this.optionsSource.AutoSize = true;
			this.optionsSource.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.optionsSource.Location = new System.Drawing.Point(174, 16);
			this.optionsSource.Name = "optionsSource";
			this.optionsSource.Size = new System.Drawing.Size(144, 21);
			this.optionsSource.TabIndex = 1014;
			this.optionsSource.Text = "<options source>";
			// 
			// elementHost1
			// 
			this.elementHost1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.elementHost1.Location = new System.Drawing.Point(2, 42);
			this.elementHost1.Name = "elementHost1";
			this.elementHost1.Size = new System.Drawing.Size(673, 528);
			this.elementHost1.TabIndex = 3;
			this.elementHost1.Text = "elementHost1";
			this.elementHost1.ChildChanged += new System.EventHandler<System.Windows.Forms.Integration.ChildChangedEventArgs>(this.elementHost1_ChildChanged);
			this.elementHost1.Child = this.optionsUI;
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(304, 595);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(116, 13);
			this.linkLabel1.TabIndex = 1015;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Report Issue on Github";
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// OptionsForm
			// 
			this.AcceptButton = this.ok;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(679, 612);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.optionsSource);
			this.Controls.Add(this.export);
			this.Controls.Add(this.import);
			this.Controls.Add(this.enableJSLint);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.ok);
			this.Controls.Add(this.elementHost1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionsForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Load += new System.EventHandler(this.OptionsForm_Load);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OptionsForm_KeyPress);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox enableJSLint;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
		private OptionsUI optionsUI;
        private System.Windows.Forms.OpenFileDialog importFileDialog;
        private System.Windows.Forms.SaveFileDialog exportFileDialog;
        private System.Windows.Forms.Button import;
		private System.Windows.Forms.Button export;
		private System.Windows.Forms.Label optionsSource;
		private System.Windows.Forms.LinkLabel linkLabel1;
    }
}