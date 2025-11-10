using Labb3_GUI.Command;
using Labb3_GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3_GUI.ViewModels
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
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


        public DelegateCommand NewQuestionCommand { get; }
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
            var pack = ActivePack;
            if (pack == null || SelectedQuestion == null)
                return;

            pack.Questions.Remove(SelectedQuestion);

            SelectedQuestion = null;

        }

        public bool CanDeleteQuestion(object? args)
        {
            return SelectedQuestion != null;
        }

        private void CreateNewQuestion()
        {
            var pack = ActivePack;
        
            SelectedQuestion = new Question("");

            pack.Questions.Add(SelectedQuestion);

            _mainWindowViewModel?.SavePacksToJason();

        }

    }
}
