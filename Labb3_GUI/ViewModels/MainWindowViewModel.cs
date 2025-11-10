using Labb3_GUI.Command;
using Labb3_GUI.Data;
using Labb3_GUI.Dialogs;
using Labb3_GUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Labb3_GUI.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly JsonPackService jsonService;
        public MainWindowViewModel()
        {
            jsonService = new JsonPackService();

            var loadedPacks = jsonService.LoadPacks();

            Packs = new ObservableCollection<QuestionPackViewModel>(loadedPacks.Select(p => new QuestionPackViewModel(p)));

            SelectedDifficulty = Difficulties[1];
            PlayerViewModel = new PlayerViewModel(this);
            ConfigurationViewModel = new ConfigurationViewModel(this);

            SelectPackCommand = new DelegateCommand(SelectPack);
            OpenCreatePackCommand = new DelegateCommand(OpenCreatePack, CanOpenCreatePack);
            SaveNewPackCommand = new DelegateCommand(SaveNewPack);
            ExitCommand = new DelegateCommand(ExitApp);

        }

        public ObservableCollection<QuestionPackViewModel> Packs { get; } = new();

        public void SavePacksToJson()
        {
            var models = Packs.Select(p => p.Pack).ToList();
            jsonService.SavePacks(models);
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


        public DelegateCommand SelectPackCommand { get; }
        public DelegateCommand OpenCreatePackCommand { get; }
        public DelegateCommand SaveNewPackCommand { get; }
        public DelegateCommand ExitCommand { get; }

        public Action CloseDialog;
        public Action OpenDialog;


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

        private void ExitApp(object? args)
        {
            try
            {
                SavePacksToJson();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Kunde inte spara: " + ex.Message);
            }

            System.Windows.Application.Current.Shutdown();
        }

        private void SaveNewPack(object? args)
        {
            Packs.Add(ActivePack);
            SavePacksToJson();

            CloseDialog?.Invoke();
        }

        private void SelectPack(object? args)
        {
            if (args is QuestionPackViewModel pack)
                ActivePack = pack;
        }

        private void OpenCreatePack(object obj)
        {
            var newPack = new QuestionPackViewModel(new QuestionPack(""));
            ActivePack = newPack;

            var dialog = new CreateNewPackDialog();
            OpenDialog?.Invoke();

        }

        private bool CanOpenCreatePack(object? args)
        {
            return true;
            //return CreatePackName.Length > 0;
        }


    }
}
