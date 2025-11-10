using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labb3_GUI.Command;
using Labb3_GUI.Models;

namespace Labb3_GUI.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<QuestionPackViewModel> Packs { get; } = new();


        private void SelectPack(object? parameter)
        {
            if (parameter is QuestionPackViewModel pack)
                ActivePack = pack;
        }

        private QuestionPackViewModel _activePack;
        public QuestionPackViewModel ActivePack
        {
            get => _activePack;
            set
            {
                _activePack = value;
                RaisePropertyChanged();
                PlayerViewModel?.RaisePropertyChanged(nameof(PlayerViewModel.ActivePack));
                ConfigurationViewModel?.RaisePropertyChanged(nameof(ConfigurationViewModel.ActivePack));
            }
        }

        public PlayerViewModel? PlayerViewModel { get; }
        public ConfigurationViewModel? ConfigurationViewModel { get; }


        public ObservableCollection<Difficulty> Difficulties { get; } = new ObservableCollection<Difficulty>(new List<Difficulty>()
        {
            Difficulty.Easy,
            Difficulty.Medium,
            Difficulty.Hard

        });



        public MainWindowViewModel()
        {
            Packs = new ObservableCollection<QuestionPackViewModel>();


            SelectedDifficulty = Difficulties[1];

            SelectPackCommand = new DelegateCommand(SelectPack);
            PlayerViewModel = new PlayerViewModel(this);
            ConfigurationViewModel = new ConfigurationViewModel(this);
            CreateNewPackCommand = new DelegateCommand(CreatePack, CanCreatePack);

            var demoPack = new QuestionPackViewModel(new QuestionPack("My Question Pack"));
            demoPack.Questions.Add(new Question("Vad är 1 + 1?", "2", "5", "1", "11"));
            demoPack.Questions.Add(new Question("Vad heter Japans huvudstad?", "Tokyo", "Tuoku", "Okinawa", "Sapporo"));
            Packs.Add(demoPack);
            ActivePack = demoPack;

        }

        public DelegateCommand SelectPackCommand { get; }
        public DelegateCommand CreateNewPackCommand { get; }


        private Difficulty _selectedDifficulty;
        public Difficulty SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                _selectedDifficulty = value;
                RaisePropertyChanged();
            }
        }

        private string _createPackName = "";

        public Action CloseDialog;

        public string CreatePackName
        {
            get { return _createPackName; }
            set
            {
                _createPackName = value;
                RaisePropertyChanged();
                CreateNewPackCommand.RaiseCanExecuteChanged();
            }
        }

        private int _createTimeLimit;

        public int CreateTimeLimit
        {
            get { return _createTimeLimit; }
            set
            {
                _createTimeLimit = value;
                RaisePropertyChanged();
                CreateNewPackCommand.RaiseCanExecuteChanged();
            }
        }



        private bool CanCreatePack(object? args)
        {
            return CreatePackName.Length > 0;
        }

        private void CreatePack(object obj)
        {
            var newPack = new QuestionPackViewModel(new QuestionPack(CreatePackName, SelectedDifficulty, CreateTimeLimit));

            Packs.Add(newPack);
            ActivePack = newPack;

            CloseDialog?.Invoke();
        }

    }
}
