using Labb3_GUI.Command;
using Labb3_GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Labb3_GUI.ViewModels
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        public MainWindowViewModel? _mainWindowViewModel { get; }
     
        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this._mainWindowViewModel = mainWindowViewModel;
            DeleteQuestionCommand = new DelegateCommand(DeleteQuestion, CanDeleteQuestion);
            NewQuestionCommand = new DelegateCommand(_ =>CreateNewQuestion());

            if (_mainWindowViewModel != null)
            {
                _mainWindowViewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(_mainWindowViewModel.ActivePack))
                    {
                        RaisePropertyChanged(nameof(ActivePack));
                        RaisePropertyChanged(nameof(ActivePack.Questions));
                    }
                };
            }

        }

        public bool IsEditingQuestion => SelectedQuestion != null;


        public DelegateCommand NewQuestionCommand  { get; }
        public DelegateCommand DeleteQuestionCommand { get; }



        private Question _selectedQuestion;
        public Question SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                RaisePropertyChanged();

                DeleteQuestionCommand.RaiseCanExecuteChanged();
                               
            }
        }

        public void DeleteQuestion(object obj)
        {
            //var pack = ActivePack;
            if (ActivePack == null || SelectedQuestion == null)
                return;

            ActivePack.Questions.Remove(SelectedQuestion);

            SelectedQuestion = null;

        }

        public bool CanDeleteQuestion(object? args)
        {
            return SelectedQuestion != null;
        }

        private void CreateNewQuestion()
        {

            if(ActivePack == null)
            {
                {
                    MessageBox.Show(
                        "No question pack is selected.\nPlease select a pack or create a new one before adding new questions.",
                        "Ooops!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }
            }
        
            SelectedQuestion = new Question("");

            ActivePack.Questions.Add(SelectedQuestion);

            _mainWindowViewModel.SavePacksToJson();

        }

    }
}
