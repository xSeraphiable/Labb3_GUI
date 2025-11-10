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
            SaveQuestionCommand = new DelegateCommand(SaveQuestion, CanSaveQuestion);
            DeleteQuestionCommand = new DelegateCommand(DeleteQuestion, CanDeleteQuestion);
            NewQuestionCommand = new DelegateCommand(_ => StartNewQuestion());

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
        public bool IsEditingQuestion => EditableQuestion != null;
        
        public DelegateCommand NewQuestionCommand { get; }
        public DelegateCommand SaveQuestionCommand { get; }
        public DelegateCommand DeleteQuestionCommand { get; }


        private Question _editableQuestion;
        public Question EditableQuestion
        {
            get => _editableQuestion;
            set
            {
                _editableQuestion = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsEditingQuestion));
            }
        }

        private Question _selectedQuestion;
        public Question SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                RaisePropertyChanged();

                if (value != null)
                {
                    EditableQuestion = new Question(
                        value.Query,
                        value.CorrectAnswer,
                        value.IncorrectAnswers[0],
                        value.IncorrectAnswers[1],
                        value.IncorrectAnswers[2]);
                }
                else
                {
                    EditableQuestion = null;
                }

                DeleteQuestionCommand.RaiseCanExecuteChanged();
                SaveQuestionCommand.RaiseCanExecuteChanged();
            }
        }


        public void DeleteQuestion(object obj)
        {
            var pack = ActivePack;
            if (pack == null || SelectedQuestion == null)
                return;

            pack.Questions.Remove(SelectedQuestion);

            SelectedQuestion = null;
            EditableQuestion = null;
                       
        }

        public bool CanDeleteQuestion(object? args)
        {
            return SelectedQuestion != null;
        }

        private void StartNewQuestion()
        {
            SelectedQuestion = null;
            EditableQuestion = new Question("", "", "", "", "");
        }


        public void SaveQuestion(object obj)
        {
            var pack = _mainWindowViewModel?.ActivePack;
            if (pack == null || EditableQuestion == null) return;

            if (SelectedQuestion != null)
            {
                SelectedQuestion.Query = EditableQuestion.Query;
                SelectedQuestion.CorrectAnswer = EditableQuestion.CorrectAnswer;
                SelectedQuestion.IncorrectAnswers = EditableQuestion.IncorrectAnswers.ToArray();
            }
            else
            {
                pack.Questions.Add(new Question(
                EditableQuestion.Query,
                EditableQuestion.CorrectAnswer,
                EditableQuestion.IncorrectAnswers.ToArray()));
            }


            EditableQuestion = null;
            SelectedQuestion = null;
            
        }

        public bool CanSaveQuestion(object? args) => true;


    }
}
