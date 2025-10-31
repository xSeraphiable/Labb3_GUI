using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labb3_GUI.Models;

namespace Labb3_GUI.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<MainWindowViewModel> Packs { get; } = new();

        private QuestionPackViewModel _activePack;

        public QuestionPackViewModel ActivePack
        {
            get => _activePack;
            set
            {
                _activePack = value;
                RaisePropertyChanged();
                PlayerViewModel?.RaisePropertyChanged(nameof(PlayerViewModel.ActivePack));
            }
        }

        public PlayerViewModel? PlayerViewModel { get; set; }
        public ConfigurationViewModel? ConfigurationViewModel { get; set; }

        public MainWindowViewModel()
        {
            PlayerViewModel = new PlayerViewModel(this);
            ConfigurationViewModel = new ConfigurationViewModel(this);

            var pack = new QuestionPack("My Question Pack");
            ActivePack = new QuestionPackViewModel(pack);
            ActivePack.Questions.Add(new Question($"Vad är 1 + 1?", "2", "5", "1", "11"));
            ActivePack.Questions.Add(new Question($"Vad heter Japans huvudstad?", "Tokyo", "Tuoku", "Okinawa", "Sapporo"));
            ActivePack.Questions.Add(new Question($"Vad heter Ellies pappa?", "Oryx", "Kira", "Obelix", "Matysh"));
        }

       
    }
}
