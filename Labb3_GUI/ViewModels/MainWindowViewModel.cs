using Labb3_GUI.Command;
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
        public MainWindowViewModel()
        {
            GetPath();
            GetPacksFromJson();


            Packs = new ObservableCollection<QuestionPackViewModel>();
            SelectedDifficulty = Difficulties[1];

            SelectPackCommand = new DelegateCommand(SelectPack);
            PlayerViewModel = new PlayerViewModel(this);
            ConfigurationViewModel = new ConfigurationViewModel(this);
            OpenCreatePackCommand = new DelegateCommand(OpenCreatePack, CanOpenCreatePack);
            SaveNewPackCommand = new DelegateCommand(SaveNewPack);

            var demoPack = new QuestionPackViewModel(new QuestionPack("My Question Pack"));
            demoPack.Questions.Add(new Question("Vad är 1 + 1?", "2", "5", "1", "11"));
            demoPack.Questions.Add(new Question("Vad heter Japans huvudstad?", "Tokyo", "Tuoku", "Okinawa", "Sapporo"));
            Packs.Add(demoPack);
            ActivePack = demoPack;

        }

        private string packPath;
        private void GetPath()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            packPath = path + "/questionpack.json";

        }

        private void GetPacksFromJson()
        {


            if (File.Exists(packPath))
            {
                var json = File.ReadAllText(packPath);
                var options = new JsonSerializerOptions()
                {
                    UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
                };

                var data = JsonSerializer.Deserialize<List<QuestionPackViewModel>>(json, options);

                foreach (var q in data)
                {
                    Packs.Add(q);
                }
            }

            else
            {
                File.Create(packPath).Dispose();
            }



            //Deserialize filen
            //Lägg till i Packs

        }

        public void SavePacksToJason()
        {

            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                //IncludeFields = true,
                //IgnoreReadOnlyProperties = true,
                //ReferenceHandler = ReferenceHandler.Preserve
            };

            var json = JsonSerializer.Serialize(Packs, options);

            //gör

            File.WriteAllText(packPath, json);

            //töm json
            //serialize
            //spara ny
        }

        public ObservableCollection<QuestionPackViewModel> Packs { get; } = new();

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

        private void SaveNewPack(object? args)
        {
            Packs.Add(ActivePack);

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
