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
        private Question _selectedQuestion;

        public Question SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                if (value != null)
                {
                    Query = value.Query;
                    CorrectAnswer = value.CorrectAnswer;
                    if (value.IncorrectAnswers.Length >= 3)
                    {
                        Answer1 = value.IncorrectAnswers[0];
                        Answer2 = value.IncorrectAnswers[1];
                        Answer3 = value.IncorrectAnswers[2];
                    }
                }
                RaisePropertyChanged();
                DeleteQuestionCommand.RaiseCanExecuteChanged();
            }
        }


        private readonly MainWindowViewModel? _mainWindowViewModel;
        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }
        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this._mainWindowViewModel = mainWindowViewModel;
            AddQuestionCommand = new DelegateCommand(CreateQuestion, CanCreateQuestion);
            DeleteQuestionCommand = new DelegateCommand(DeleteQuestion, CanDeleteQuestion);
        }

        public DelegateCommand AddQuestionCommand { get; }
        public DelegateCommand DeleteQuestionCommand { get; }
        public string Query { get; set; } = "";
        public string CorrectAnswer { get; set; } = "";
        public string Answer1 { get; set; } = "";
        public string Answer2 { get; set; } = "";
        public string Answer3 { get; set; } = "";

        public void DeleteQuestion(object obj)
        {
            _mainWindowViewModel?.ActivePack?.Questions.Remove(SelectedQuestion);

            SelectedQuestion = null;
            Query = Answer1 = Answer2 = Answer3 = CorrectAnswer = string.Empty;

            RaisePropertyChanged(nameof(Query));
            RaisePropertyChanged(nameof(CorrectAnswer));
            RaisePropertyChanged(nameof(Answer1));
            RaisePropertyChanged(nameof(Answer2));
            RaisePropertyChanged(nameof(Answer3));
        }
        public bool CanDeleteQuestion(object? args)
        {
            return SelectedQuestion != null;

        }


        public void CreateQuestion(object obj)
        {
            var newQuestion = new Question(Query, CorrectAnswer, Answer1, Answer2, Answer3);
            _mainWindowViewModel?.ActivePack?.Questions.Add(newQuestion);

            Query = Answer1 = Answer2 = Answer3 = CorrectAnswer = string.Empty;

            RaisePropertyChanged(nameof(Query));
            RaisePropertyChanged(nameof(CorrectAnswer));
            RaisePropertyChanged(nameof(Answer1));
            RaisePropertyChanged(nameof(Answer2));
            RaisePropertyChanged(nameof(Answer3));
        }

        public bool CanCreateQuestion(object? args)
        {
            return true;
        }

    }
}
