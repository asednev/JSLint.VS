using System;
using JSLint.Framework.OptionClasses;
using JSLint.UI.OptionsUI.HelperClasses;

namespace JSLint.UI.OptionsUI.ViewModel
{
    public class LintBooleanSettingViewModel : ViewModelBase
    {
        private JSLintOptions _jslintOptions;
        private LintBooleanSettingModel _model;
        public string Tooltip
        {
            get
            {
                return String.Format("{0}: {1}", _model.JSName, _model.Tooltip);
            }
        }
        public string SettingName { get { return _model.Label; } }

        public bool DefaultOn
        {
            get
            {
                return _model.DefaultOn;
            }
        }

        public bool On
        {
            get
            {
                if (!_jslintOptions.BoolOptions2.ContainsKey(_model.JSName))
                {
                    _jslintOptions.BoolOptions2[_model.JSName] = _model.DefaultOn;
                }
                return _jslintOptions.BoolOptions2[_model.JSName];
            }
            set { _jslintOptions.BoolOptions2[_model.JSName] = value; OnPropertyChanged("On"); }
        }

        public LintBooleanSettingViewModel(LintBooleanSettingModel model, JSLintOptions jslintOptions)
        {
            _model = model;
            _jslintOptions = jslintOptions;
        }
    }
}
