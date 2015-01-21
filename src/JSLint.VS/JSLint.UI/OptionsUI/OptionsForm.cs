using System;
using System.Diagnostics;
using System.Windows.Forms;
using JSLint.Framework.OptionClasses;

namespace JSLint.UI.OptionsUI
{
    public partial class OptionsForm : Form
    {
        private static string _title = string.Format(
                "JavaScript Linter for Visual Studio v{0}",
                typeof(OptionsForm).Assembly.GetName().Version.ToString());

        private Options _options;
		private IOptionsProvider _provider;
        public OptionsForm()
        {
			_provider = OptionsProviderRegistry.CurrentOptionsProvider;
			_options = _provider.GetOptions();
            InitializeComponent();
			optionsUI.SetOptions(_options);
        }

		public String OptionsSourceName
		{
			get { return optionsSource.Text; }
			set { optionsSource.Text = value; }
		}

        private void LoadOptions(Options options)
        {
            optionsUI.LoadOptions(options);

            optionsUI.IsEnabled =
                enableJSLint.Checked
                = options.Enabled;
        }

        private void SaveOptions(Options dest)
        {
            dest.Enabled = enableJSLint.Checked;
            optionsUI.SaveOptions(dest);
        }

        private void enableJSLint_CheckedChanged(object sender, EventArgs e)
        {
            optionsUI.IsEnabled = enableJSLint.Checked;
        }

        private void OptionsForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (int)Keys.Escape)
            {
                Hide();
            }
        }

        private void import_Click(object sender, EventArgs e)
        {
            if (importFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Options options = OptionsProviderRegistry.Import(importFileDialog.FileName);
                    LoadOptions(options);
                }
                catch (Exception x)
                {
                    MessageBox.Show(
                        x.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void export_Click(object sender, EventArgs e)
        {
            if (exportFileDialog.ShowDialog() == DialogResult.OK)
            {
                Options exportedOptions = new Options();
                SaveOptions(exportedOptions);
                OptionsProviderRegistry.Export(exportedOptions, exportFileDialog.FileName);
            }
        }

		//private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		//{
		//	this.linkLabel1.LinkVisited = true;

		//	Process.Start("http://www.jslint.com/");
		//}

		//private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		//{
		//	this.linkLabel2.LinkVisited = true;

		//	Process.Start("http://jslint4vs2010.codeplex.com/");
		//}

        private void ok_Click(object sender, EventArgs e)
        {
            SaveOptions(_options);

			try
			{
				OptionsProviderRegistry.SaveChanges(_options);
			}
			catch (Exception ex)
			{
				String msg = "Unable to save configuration changes: " + Environment.NewLine
								+ ex.Message;
				MessageBox.Show(msg, "Error Saving Configuration");
			}

            Hide();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            Text = _title;

            KeyPreview = true;

            LoadOptions(_options);
		}

		private void elementHost1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
		{

		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			linkLabel1.LinkVisited = true;

			Process.Start("https://github.com/asednev/JSLint.VS/issues");
		}

    }
}

