using System.Collections.Generic;
using System.Collections.ObjectModel;
using JSLint.Framework.OptionClasses;
using JSLint.UI.OptionsUI.HelperClasses;

namespace JSLint.UI.OptionsUI.ViewModel
{
    public class OptionsViewModel : ViewModelBase
    {
        public ObservableCollection<LinterViewModel> Linters
        {
            get;
            set;
        }

        private LinterViewModel _selected = null;

        private JSLintOptions _options;

        public LinterViewModel Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                _options.SelectedLinter = _selected.LinterType;
                OnPropertyChanged("Selected");
                Selected.TriggerSettingsChange();
            }
        }

        internal void ResetLinterSettings()
        {
            Selected.Reset();
        }

        internal void ClearLinterSettings()
        {
            Selected.Clear();
        }

        public OptionsViewModel(JSLintOptions jslintoptions)
        {
            this._options = jslintoptions;

            Linters = new ObservableCollection<LinterViewModel>();
            Linters.Add(new LinterViewModel(new LinterModel("JSLint", Framework.OptionClasses.Linters.JSLint), jslintoptions));
            //Linters.Add(new LinterViewModel(new LinterModel("JSLint Old", Framework.OptionClasses.Linters.JSLintOld), jslintoptions));
            Linters.Add(new LinterViewModel(new LinterModel("JSHint", Framework.OptionClasses.Linters.JSHint) 
                { HasUnusedVariableOptionBakedIn = true, HasQuotMarkOption = true, HasMaxComplexityOptions = true }, jslintoptions));
            LoadSelected();
        }

        private void LoadSelected()
        {
            foreach (LinterViewModel lvm in Linters)
            {
                if (lvm.LinterType == _options.SelectedLinter)
                {
                    Selected = lvm;
                    break;
                }
            }
        }

        internal void LoadLinterSettings(Linters linter, SerializableDictionary<string, bool> boolOptions)
        {
            if (boolOptions != this._options.BoolOptions2)
            {
                this._options.BoolOptions2.Clear();
                foreach (KeyValuePair<string, bool> option in boolOptions)
                {
                    this._options.BoolOptions2[option.Key] = option.Value;
                }
				
				this.


                Selected.TriggerSettingsChange();
            }
            _options.SelectedLinter = linter;
            LoadSelected();
        }

        internal Linters SaveLinterSettings(SerializableDictionary<string, bool> boolOptions)
        {
            //TODO HACK
            // really the view model should be capable of producing options from itself
            if (boolOptions != this._options.BoolOptions2)
            {
                boolOptions.Clear();
                foreach (KeyValuePair<string, bool> option in this._options.BoolOptions2)
                {
                    boolOptions[option.Key] = option.Value;
                }
            }
            return _options.SelectedLinter;
        }
    }
}
