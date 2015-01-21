using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JSLint.Framework.OptionClasses;
using JSLint.UI.OptionsUI.ViewModel;

namespace JSLint.UI.OptionsUI
{
	public partial class OptionsUI : UserControl
	{
		internal static int SelectedTabIndex = 0;

		private OptionsViewModel _optionsVM;

		private Options _options;

		public OptionsUI()
		{
			InitializeComponent();
		}

		public void SetOptions(Options options)
		{
			_options = options;
			DataContext = _optionsVM = new OptionsViewModel(options.JSLintOptions);
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			LoadOptions(_options);
		}

		private void tabControl1_Loaded(object sender, RoutedEventArgs e)
		{
			tabControl1.SelectedIndex = SelectedTabIndex;
		}

		private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (tabControl1.IsLoaded)
			{
				SelectedTabIndex = tabControl1.SelectedIndex;
			}
		}

		private void numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			string s = e.Text;
			if (string.IsNullOrEmpty(s))
			{
				e.Handled = true;
			}
			else
			{
				for (int i = 0; i < s.Length; i++)
				{
					if (!char.IsDigit(s[i]))
					{
						e.Handled = true;
						return;
					}
				}
			}
		}

		private void Reset()
		{
			//LoadIntoCheckboxes(JSLintOptions.DefaultBoolOptions);

			indentSize.Text = "4";
			maxlen.Text = "90";
			predefined.Text = string.Empty;
			warnOnUnused.IsChecked = true;
		}

		private void reset_Click(object sender, RoutedEventArgs e)
		{
			Reset();
			_optionsVM.ResetLinterSettings();
		}

		private void clear_Click(object sender, RoutedEventArgs e)
		{
			//LoadIntoCheckboxes((JSLintBoolOption)0);
			_optionsVM.ClearLinterSettings();

			indentSize.Text =
				maxlen.Text =
			predefined.Text = string.Empty;
			warnOnUnused.IsChecked = false;
			runOnSave.IsChecked = false;
		}

		public void LoadOptions(Options options)
		{
			outputAsError.IsChecked = options.ErrorCategory == ErrorCategory.Error;
			outputAsWarning.IsChecked = options.ErrorCategory == ErrorCategory.Warning;
			outputAsMessage.IsChecked = options.ErrorCategory == ErrorCategory.Message;
			outputAsTask.IsChecked = options.ErrorCategory == ErrorCategory.Task;

			todoAsError.IsChecked = options.TODOCategory == ErrorCategory.Error;
			todoAsWarning.IsChecked = options.TODOCategory == ErrorCategory.Warning;
			todoAsMessage.IsChecked = options.TODOCategory == ErrorCategory.Message;
			todoAsTask.IsChecked = options.TODOCategory == ErrorCategory.Task;
			findTODOs.IsChecked = options.TODOEnabled;

			runOnSave.IsChecked = options.RunOnSave;

			runOnBuild.IsChecked = options.RunOnBuild;
			cancelBuildOnError.IsChecked = options.CancelBuildOnError;

			includeJS.IsChecked = (options.BuildFileTypes & IncludeFileType.JS) == IncludeFileType.JS;
			includeCSS.IsChecked = (options.BuildFileTypes & IncludeFileType.CSS) == IncludeFileType.CSS;
			includeHTML.IsChecked = (options.BuildFileTypes & IncludeFileType.HTML) == IncludeFileType.HTML;
			onSaveJs.IsChecked = (options.SaveFileTypes & IncludeFileType.JS) == IncludeFileType.JS;
			onSaveCss.IsChecked = (options.SaveFileTypes & IncludeFileType.CSS) == IncludeFileType.CSS;
			onSaveHtml.IsChecked = (options.SaveFileTypes & IncludeFileType.HTML) == IncludeFileType.HTML;

			fakeAtCharset.IsChecked = options.FakeCSSCharset;

			ignoreErrorStart.Text = options.IgnoreErrorStart;
			ignoreErrorEnd.Text = options.IgnoreErrorEnd;
			IgnoreErrorLine.Text = options.IgnoreErrorLine;

			JSLintOptions jslint = options.JSLintOptions;

			_optionsVM.LoadLinterSettings(jslint.SelectedLinter, jslint.BoolOptions2);

			warnOnUnused.IsChecked = jslint.ErrorOnUnused;
			indentSize.Text = jslint.IndentSize.ToString();
			maxlen.Text = jslint.MaxLength.ToString();

			if (jslint.PreDefined != null && jslint.PreDefined.Count > 0)
			{
				predefined.Text = string.Join(", ", jslint.PreDefined);
			}
		}

		public void SaveOptions(Options dest)
		{
			if (outputAsError.IsChecked.GetValueOrDefault(false))
			{
				dest.ErrorCategory = ErrorCategory.Error;
			}
			else if (outputAsWarning.IsChecked.GetValueOrDefault(false))
			{
				dest.ErrorCategory = ErrorCategory.Warning;
			}
			else if (outputAsMessage.IsChecked.GetValueOrDefault(false))
			{
				dest.ErrorCategory = ErrorCategory.Message;
			}
			else
			{
				dest.ErrorCategory = ErrorCategory.Task;
			}

			if (todoAsError.IsChecked.GetValueOrDefault(false))
			{
				dest.TODOCategory = ErrorCategory.Error;
			}
			else if (todoAsWarning.IsChecked.GetValueOrDefault(false))
			{
				dest.TODOCategory = ErrorCategory.Warning;
			}
			else if (todoAsMessage.IsChecked.GetValueOrDefault(false))
			{
				dest.TODOCategory = ErrorCategory.Message;
			}
			else
			{
				dest.TODOCategory = ErrorCategory.Task;
			}

			dest.TODOEnabled = findTODOs.IsChecked.GetValueOrDefault(false);

			dest.RunOnBuild = runOnBuild.IsChecked.HasValue && runOnBuild.IsChecked.Value;
			dest.CancelBuildOnError = cancelBuildOnError.IsChecked.HasValue && cancelBuildOnError.IsChecked.Value;

			dest.BuildFileTypes = includeJS.IsChecked.GetValueOrDefault(true) ? IncludeFileType.JS : (IncludeFileType)0;

			if (includeCSS.IsChecked.GetValueOrDefault(false))
			{
				dest.BuildFileTypes |= IncludeFileType.CSS; 
			}

			if (includeHTML.IsChecked.GetValueOrDefault(false))
			{
				dest.BuildFileTypes |= IncludeFileType.HTML;
			}

			dest.SaveFileTypes = onSaveJs.IsChecked.GetValueOrDefault(true) ? IncludeFileType.JS : (IncludeFileType)0;
			if (onSaveCss.IsChecked.GetValueOrDefault(false))
			{
				dest.SaveFileTypes |= IncludeFileType.CSS; 
			}

			if (onSaveHtml.IsChecked.GetValueOrDefault(false))
			{
				dest.SaveFileTypes |= IncludeFileType.HTML;
			}

			dest.FakeCSSCharset = fakeAtCharset.IsChecked.GetValueOrDefault(false);

			dest.IgnoreErrorStart = ignoreErrorStart.Text;
			dest.IgnoreErrorEnd = ignoreErrorEnd.Text;
			dest.IgnoreErrorLine = IgnoreErrorLine.Text;
			dest.RunOnSave = runOnSave.IsChecked.GetValueOrDefault(false);

			var jslint = dest.JSLintOptions;

			jslint.SelectedLinter = _optionsVM.SaveLinterSettings(jslint.BoolOptions2);

			if (indentSize.Text.Length == 0)
			{
				jslint.IndentSize = null;
			}
			else
			{
				jslint.IndentSize = int.Parse(indentSize.Text);
			}

			if (maxlen.Text.Length == 0)
			{
				jslint.MaxLength = null;
			}
			else
			{
				jslint.MaxLength = int.Parse(maxlen.Text);
			}

			if (!string.IsNullOrWhiteSpace(predefined.Text))
			{
				jslint.PreDefined = new List<string>(predefined.Text.Split(new string[] { ",", " ", "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries));
			}
			else 
			{
				jslint.PreDefined = null;
			}

            if (jslint.SelectedLinter == Linters.JSHint)
            {
                jslint.ErrorOnUnused = false;
            }
            else
            {
                jslint.ErrorOnUnused = warnOnUnused.IsChecked.GetValueOrDefault(true);
            }
		}
	}
}

