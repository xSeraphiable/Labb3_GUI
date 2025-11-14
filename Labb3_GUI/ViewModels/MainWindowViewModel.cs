using Labb3_GUI.Command;
using Labb3_GUI.Data;
using Labb3_GUI.Dialogs;
using Labb3_GUI.Models;
using Labb3_GUI.Services;

using System.Collections.ObjectModel;

using System.Windows;



namespace Labb3_GUI.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly AppNavigator _navigator;
        private readonly JsonPackService _jsonService;

        private WindowState _windowState = WindowState.Normal;
        private WindowStyle _windowStyle = WindowStyle.SingleBorderWindow;
        private bool _isFullScreen = false;

        public MainWindowViewModel()
        {
            _jsonService = new JsonPackService();

            LoadPacks();

            PlayerViewModel = new PlayerViewModel(this);
            ConfigurationViewModel = new ConfigurationViewModel(this);

            SelectPackCommand = new DelegateCommand(SelectPack);
            OpenCreatePackCommand = new DelegateCommand(OpenCreatePack, CanEditPack);
            SaveNewPackCommand = new DelegateCommand(SaveNewPack);
            EditPackCommand = new DelegateCommand(EditPack, CanEditPack);
            ExitCommand = new DelegateCommand(ExitApp);
            DeletePackCommand = new DelegateCommand(DeletePack, CanDeletePack);
            ShowPlayerViewCommand = new DelegateCommand(ShowPlayerView);
            ShowConfigViewCommand = new DelegateCommand(ShowConfigView);
            ToggleFullscreenCommand = new DelegateCommand(ToggleFullScreen);
            CancelNewPackCommand = new DelegateCommand(CancelNewPack);

            _navigator = new AppNavigator();
            _navigator.NavigatingAwayFromQuiz += PlayerViewModel.StopQuiz;
            _navigator.ViewChanged += view => CurrentView = view;
            _navigator.NavigateTo(ConfigurationViewModel);

            ActivePack = Packs.FirstOrDefault();
        }

        public PlayerViewModel? PlayerViewModel { get; }
        public ConfigurationViewModel? ConfigurationViewModel { get; }


        public ObservableCollection<QuestionPackViewModel> Packs { get; private set; } = new();


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

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                RaisePropertyChanged();

                OpenCreatePackCommand.RaiseCanExecuteChanged();
                DeletePackCommand.RaiseCanExecuteChanged();
                EditPackCommand.RaiseCanExecuteChanged();

            }
        }

        public void SavePacksToJson()
        {
            var models = Packs.Select(p => p.Pack).ToList();
            _jsonService.SavePacks(models);
        }

        private void LoadPacks()
        {
            var loaded = _jsonService.LoadPacks();
            Packs = new ObservableCollection<QuestionPackViewModel>(
                loaded.Select(p => new QuestionPackViewModel(p))
            );

            SelectedDifficulty = Difficulties[1];
        }

        public DelegateCommand SelectPackCommand { get; }
        public DelegateCommand OpenCreatePackCommand { get; }
        public DelegateCommand SaveNewPackCommand { get; }
        public DelegateCommand CancelNewPackCommand { get; }
        public DelegateCommand EditPackCommand { get; }
        public DelegateCommand ExitCommand { get; }
        public DelegateCommand DeletePackCommand { get; }
        public DelegateCommand ShowPlayerViewCommand { get; }
        public DelegateCommand ShowConfigViewCommand { get; }
        public DelegateCommand ToggleFullscreenCommand { get; }


        public Action CloseDialog;
        public Action OpenDialog;

        public ObservableCollection<Difficulty> Difficulties { get; } =
            new ObservableCollection<Difficulty>(new[] { Difficulty.Easy, Difficulty.Medium, Difficulty.Hard });


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
        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                RaisePropertyChanged();
            }
        }

        public WindowStyle WindowStyle
        {
            get => _windowStyle;
            set
            {
                _windowStyle = value;
                RaisePropertyChanged();
            }
        }
        private void ToggleFullScreen(object? args)
        {
            _isFullScreen = !_isFullScreen;
            WindowStyle = _isFullScreen ? WindowStyle.None : WindowStyle.SingleBorderWindow;
            WindowState = _isFullScreen ? WindowState.Maximized : WindowState.Normal;

            //_isFullScreen = !_isFullScreen;

            //if (_isFullScreen)
            //{
            //    WindowState = WindowState.Normal;
            //    WindowStyle = WindowStyle.None;
            //    WindowState = WindowState.Maximized;
            //}
            //else
            //{
            //    WindowStyle = WindowStyle.SingleBorderWindow;
            //    WindowState = WindowState.Normal;
            //}
        }


        private void ShowPlayerView(object? args)
        {
            PlayerViewModel.InitializeQuiz();
            _navigator.NavigateTo(PlayerViewModel);
        }

        private void ShowConfigView(object? args)
        {
            _navigator.NavigateTo(ConfigurationViewModel);
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


        private void CancelNewPack(object? args)
        {
            if (Packs.Count > 0)
            { ActivePack = Packs.Last(); }

            CloseDialog?.Invoke();
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

        private void OpenCreatePack(object args)
        {
            var newPack = new QuestionPackViewModel(new QuestionPack("<New Pack>"));
            ActivePack = newPack;

            var dialog = new CreateNewPackDialog();
            OpenDialog?.Invoke();

        }

        private void EditPack(object args)
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

        private bool CanEditPack(object? args)
        {
            return (CurrentView is ConfigurationViewModel);
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
            return ActivePack != null && Packs.Contains(ActivePack) && (CurrentView is ConfigurationViewModel);
        }


    }
}
