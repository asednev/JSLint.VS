using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using JSLint.Framework.OptionClasses;
using JSLint.UI.OptionsUI.HelperClasses;

namespace JSLint.UI.OptionsUI.ViewModel
{
    public class LinterViewModel : ViewModelBase
    {
        private LinterModel _linter = null;
        private ObservableCollection<LintBooleanSettingViewModel> _booleanSettings;
        private JSLintOptions _jslintOptions;

        public ObservableCollection<LintBooleanSettingViewModel> Settings
        {
            get { return _booleanSettings; }
        }

        public string LinterName
        {
            get { return _linter.Name; }
        }

        public Visibility QuoteMarkOptionVisible
        {
            get { return _linter.HasQuotMarkOption ? Visibility.Visible : Visibility.Hidden; }
        }

        public Visibility WarnOnUnusedVisible
        {
            get { return _linter.HasUnusedVariableOptionBakedIn ? Visibility.Hidden : Visibility.Visible; }
        }

        public Visibility MaxComplexityVisible
        {
            get { return _linter.HasMaxComplexityOptions ? Visibility.Visible : Visibility.Hidden; }
        }

        public List<string> QuoteMarkOptions
        {
            get { return new List<string>() { "On", "Off", "Single", "Double" }; }
        }

        public string SelectedQuoteMarkOption
        {
            get
            {
                return _jslintOptions.QuoteMark ?? "Off";
            }
            set
            {
                _jslintOptions.QuoteMark = value;
                OnPropertyChanged("SelectedQuoteMarkOption");
            }
        }

        public string MaxComplexity
        {
            get
            {
                if (_jslintOptions.MaxComplexity == null)
                    return string.Empty;
                return _jslintOptions.MaxComplexity.Value.ToString();
            }
            set
            {
                int val;
                if (int.TryParse(value, out val) && val != 0)
                {
                    _jslintOptions.MaxComplexity = val;
                }
                else
                {
                    _jslintOptions.MaxComplexity = null;
                }
                OnPropertyChanged("MaxComplexity");
            }
        }

        public string MaxStatements
        {
            get
            {
                if (_jslintOptions.MaxStatements == null)
                    return string.Empty;
                return _jslintOptions.MaxStatements.Value.ToString();
            }
            set
            {
                int val;
                if (int.TryParse(value, out val) && val != 0)
                {
                    _jslintOptions.MaxStatements = val;
                }
                else
                {
                    _jslintOptions.MaxStatements = null;
                }
                OnPropertyChanged("MaxStatements");
            }
        }


        public string MaxDepth
        {
            get
            {
                if (_jslintOptions.MaxDepth == null)
                    return string.Empty;
                return _jslintOptions.MaxDepth.Value.ToString();
            }
            set
            {
                int val;
                if (int.TryParse(value, out val) && val != 0)
                {
                    _jslintOptions.MaxDepth = val;
                }
                else
                {
                    _jslintOptions.MaxDepth = null;
                }
                OnPropertyChanged("MaxDepth");
            }
        }


        public string MaxParams
        {
            get
            {
                if (_jslintOptions.MaxParams == null)
                    return string.Empty;
                return _jslintOptions.MaxParams.Value.ToString();
            }
            set
            {
                int val;
                if (int.TryParse(value, out val) && val != 0)
                {
                    _jslintOptions.MaxParams = val;
                }
                else
                {
                    _jslintOptions.MaxParams = null;
                }
                OnPropertyChanged("MaxParams");
            }
        }

        internal void TriggerSettingsChange()
        {
            foreach (LintBooleanSettingViewModel vm in Settings)
            {
                vm.On = vm.On;
            }
        }

        public LinterViewModel(LinterModel linterModel, JSLintOptions jslintOptions)
        {
            _linter = linterModel;
            _jslintOptions = jslintOptions;
            _booleanSettings = new ObservableCollection<LintBooleanSettingViewModel>(GetBooleanSettings(_linter, _jslintOptions));
        }

        public IEnumerable<LintBooleanSettingViewModel> GetBooleanSettings(LinterModel linter, JSLintOptions jslintoptions)
        {
			return LintBooleanSettingModel.AllOptions
				.Where(a => ((a.LintersAppliesTo & linter.Type) > 0))
				.Select(a => new LintBooleanSettingViewModel(a, jslintoptions))
				.OrderBy(a => a.SettingName);
        }

        public void Reset()
        {
            foreach (LintBooleanSettingViewModel vm in Settings)
            {
                vm.On = vm.DefaultOn;
            }
            MaxComplexity = "";
            MaxDepth = "";
            MaxParams = "";
            MaxStatements = "";
            SelectedQuoteMarkOption = "Off";
        }

        public void Clear()
        {
            foreach (LintBooleanSettingViewModel vm in Settings)
            {
                vm.On = false;
            }
            MaxComplexity = "";
            MaxDepth = "";
            MaxParams = "";
            MaxStatements = "";
            SelectedQuoteMarkOption = "Off";
        }

        public Linters LinterType 
        {
            get
            {
                return _linter.Type;
            }
        }
    }
}
