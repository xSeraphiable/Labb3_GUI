using Labb3_GUI.Command;
using Labb3_GUI.Data;
using Labb3_GUI.Dialogs;
using Labb3_GUI.Models;
using Labb3_GUI.Visuals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


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

            CurrentView = ConfigurationViewModel;

            SelectPackCommand = new DelegateCommand(SelectPack);
            OpenCreatePackCommand = new DelegateCommand(OpenCreatePack, CanOpenCreatePack);
            SaveNewPackCommand = new DelegateCommand(SaveNewPack);
            EditPackCommand = new DelegateCommand(EditPack);
            ExitCommand = new DelegateCommand(ExitApp);
            DeletePackCommand = new DelegateCommand(DeletePack, CanDeletePack);
            ShowPlayerViewCommand = new DelegateCommand(ShowPlayerView);
            ShowConfigViewCommand = new DelegateCommand(ShowConfigView);

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
                DeletePackCommand.RaiseCanExecuteChanged();
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

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand SelectPackCommand { get; }
        public DelegateCommand OpenCreatePackCommand { get; }
        public DelegateCommand SaveNewPackCommand { get; }
        public DelegateCommand EditPackCommand { get; }
        public DelegateCommand ExitCommand { get; }
        public DelegateCommand DeletePackCommand { get; }
        public DelegateCommand ShowPlayerViewCommand { get; }
        public DelegateCommand ShowConfigViewCommand { get; }


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

        private void ShowPlayerView(object? args)
        {
            CurrentView = PlayerViewModel;
        }
        private void ShowConfigView(object? args)
        {
            CurrentView = ConfigurationViewModel;
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

        private void EditPack(object obj)
        {
            if (ActivePack == null)
            {
                {
                    MessageBox.Show(
                        "No question pack is selected.",
                        "Ooops!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }
            }

            var dialog = new PackOptionsDialog();
            OpenDialog?.Invoke();
        }

        private void DeletePack(object? args)
        {
            if (ActivePack == null) return;

            var result = MessageBox.Show(
               $"Are you sure you want to delete {ActivePack.Name}?",
               "Delete pack",
               MessageBoxButton.OKCancel,
               MessageBoxImage.Warning,
               MessageBoxResult.Cancel);

            if (result == MessageBoxResult.OK)
            {
                var index = Packs.IndexOf(ActivePack);
                Packs.Remove(ActivePack);

                if (Packs.Count > 0)
                {
                    ActivePack = Packs[Math.Min(index, Packs.Count - 1)];
                }
                else
                {
                    ActivePack = null;
                }
            }
        }

        private bool CanDeletePack(object? args)
        {
            return ActivePack != null;
        }

    }
}
