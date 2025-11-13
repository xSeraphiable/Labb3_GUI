using Labb3_GUI.Command;
using Labb3_GUI.Models;
using Labb3_GUI.Visuals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Labb3_GUI.ViewModels
{
    internal class PlayerViewModel : ViewModelBase
    {

        private readonly MainWindowViewModel? _mainWindowViewModel;
        private readonly DispatcherTimer _timer;
        private static readonly Random _rng = new Random();
        private int _timeLeft;

        public DelegateCommand AnswerCommand { get; }

        public QuestionPackViewModel? ActivePack => _mainWindowViewModel?.ActivePack;

        public ObservableCollection<Question> PlayQuestions { get; private set; }

        private Question _currentQuestion;
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                _currentQuestion = value; RaisePropertyChanged();
                RaisePropertyChanged(nameof(CurrentQuestionNumber));
            }
        }

        public int CurrentQuestionIndex { get; set; }

        public int Score { get; set; }

        public int TimeLeft
        {
            get => _timeLeft;
            set { _timeLeft = value; RaisePropertyChanged(); }
        }

        public string CurrentQuestionNumber => PlayQuestions != null && CurrentQuestion != null ?
            $"Question {PlayQuestions.IndexOf(CurrentQuestion) + 1} / {PlayQuestions.Count}"
            : "Question 0 / 0";

        public List<bool> ShowCorrectIndicator { get; set; } = new List<bool> { false, false, false, false };
        public List<bool> ShowIncorrectIndicator { get; set; } = new List<bool> { false, false, false, false };


        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            AnswerCommand = new DelegateCommand(CheckAnswer);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (TimeLeft > 0)
                TimeLeft--;
            else
            {
                _timer.Stop();
                OnTimeUp();
            }
        }

        public void StartTimer(int seconds)
        {
            TimeLeft = seconds;
            _timer.Start();
        }

        public void StopQuiz()
        {
            _timer.Stop();
            TimeLeft = 0;
        }


        private void MoveToNextQuestion()
        {
            var currentIndex = PlayQuestions.IndexOf(CurrentQuestion);
            if (currentIndex + 1 < PlayQuestions.Count)
            {
                CurrentQuestion = PlayQuestions[currentIndex + 1];
                StartTimer(ActivePack.TimeLimitInSeconds);
            }
            else
            {
                ShowResults();
            }
        }

        private void ShowResults()
        {
            var resultVm = new PlayerResultViewModel(_mainWindowViewModel, Score, PlayQuestions.Count);
            _mainWindowViewModel.CurrentView = resultVm;
        }


        private async void CheckAnswer(object? args)
        {
            var answer = args as string;
            if (answer == null || CurrentQuestion == null)
                return;

            _timer.Stop();

            int selectedIndex = CurrentQuestion.ShuffledAnswers.IndexOf(answer);
            int correctIndex = CurrentQuestion.ShuffledAnswers.IndexOf(CurrentQuestion.CorrectAnswer);

            if (selectedIndex == correctIndex)
            {
                Score++;
            }

            await ShowFeedbackIndicators(selectedIndex);

            MoveToNextQuestion();
        }

        private async void OnTimeUp()
        {
            _timer.Stop();
            await ShowFeedbackIndicators();
            MoveToNextQuestion();
        }

        private async Task ShowFeedbackIndicators(int? selectedIndex = null)
        {
            if (CurrentQuestion == null) return;

            int correctIndex = GetCorrectAnswerIndex();
            SetIndicators(correctIndex, selectedIndex);

            await Task.Delay(1800);

            ResetIndicators();
        }


        private int GetCorrectAnswerIndex() =>
            CurrentQuestion.ShuffledAnswers.IndexOf(CurrentQuestion.CorrectAnswer);

        private void SetIndicators(int correctIndex, int? selectedIndex)
        {
            for (int i = 0; i < 4; i++)
            {
                ShowCorrectIndicator[i] = (i == correctIndex);
                ShowIncorrectIndicator[i] = (selectedIndex.HasValue && selectedIndex.Value == i && i != correctIndex);
            }

            RaisePropertyChanged(nameof(ShowCorrectIndicator));
            RaisePropertyChanged(nameof(ShowIncorrectIndicator));
        }


        private void ResetIndicators()
        {
            for (int i = 0; i < 4; i++)
                ShowCorrectIndicator[i] = ShowIncorrectIndicator[i] = false;

            RaisePropertyChanged(nameof(ShowCorrectIndicator));
            RaisePropertyChanged(nameof(ShowIncorrectIndicator));
        }

        public void InitializeQuiz()
        {
            if (!CheckActivePack()) return;

            ResetScore();
            ShuffleQuestions();
            ShuffleAnswers();
            SetCurrentQuestion();
            StartTimer(ActivePack.TimeLimitInSeconds);
        }

        private bool CheckActivePack()
        {
            if (ActivePack == null)
            {
                MessageBox.Show("No question pack selected.", "Ooops!");
                return false;
            }
            return true;
        }

        private void ResetScore() => Score = 0;

        private void ShuffleQuestions()
        {
            PlayQuestions = new ObservableCollection<Question>(
                ActivePack.Questions
                          .OrderBy(q => _rng.Next())
            );
        }

        private void ShuffleAnswers()
        {
            foreach (var q in PlayQuestions)
            {
                q.ShuffledAnswers = q.IncorrectAnswers
                                      .Append(q.CorrectAnswer)
                                      .OrderBy(a => _rng.Next())
                                      .ToList();
            }
        }

        private void SetCurrentQuestion()
        {
            CurrentQuestionIndex = 0;
            CurrentQuestion = PlayQuestions.FirstOrDefault();
        }




    }
}
