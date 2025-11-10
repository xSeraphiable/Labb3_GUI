using Labb3_GUI.Command;
using Labb3_GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3_GUI.ViewModels
{
    internal class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;

        public DelegateCommand SetPackNameCommand { get; }
        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this._mainWindowViewModel = mainWindowViewModel;

            SetPackNameCommand = new DelegateCommand(SetPackName, CanSetPackName);
            DemoText = string.Empty;
        }
        private string _demoText;

        public string DemoText
        {
            get { return _demoText; }
            set
            {
                _demoText = value;
                RaisePropertyChanged();
                SetPackNameCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanSetPackName(object? args)
        {
            return DemoText.Length > 0;
        }

        private void SetPackName(object obj)
        {
            ActivePack.Name = DemoText;
        }
    }
}
