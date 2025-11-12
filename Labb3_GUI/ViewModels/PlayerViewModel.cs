using Labb3_GUI.Command;
using Labb3_GUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Labb3_GUI.ViewModels
{
    internal class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;

        public DelegateCommand SetPackNameCommand { get; }
        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this._mainWindowViewModel = mainWindowViewModel;


            AnswerCommand = new DelegateCommand(CheckAnswer);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;


        }

        public DelegateCommand AnswerCommand { get; }

        public ObservableCollection<Question> PlayQuestions { get; private set; }


        private Question _currentQuestion;

        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set { _currentQuestion = value; RaisePropertyChanged(); }
        }

        public int CurrentQuestionIndex { get; set; }

        private static readonly Random _rng = new Random();

        private readonly DispatcherTimer _timer;

        public int Score { get; set; }

        public List<bool> ShowCorrectIndicator { get; set; } = new List<bool> { false, false, false, false };
        public List<bool> ShowIncorrectIndicator { get; set; } = new List<bool> { false, false, false, false };

        private int _timeLeft;

        public int TimeLeft
        {
            get { return _timeLeft; }
            set { _timeLeft = value; RaisePropertyChanged(); }
        }
  
        private void Timer_Tick(object? sender, EventArgs e)
        {

            if (TimeLeft > 0)
            {
                TimeLeft--;
            }
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


        private void ShowResults()
        {
            MessageBox.Show($"Du fick {Score} rätt av {PlayQuestions.Count} frågor.");
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
                ShowResults();
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

        public void InitializeQuiz()
        {
            if (ActivePack == null)
            {
                MessageBox.Show("No question pack selected.", "Ooops!");
                return;
            }

            var shuffled = _mainWindowViewModel.ActivePack.Questions.ToList();

            shuffled = shuffled.OrderBy(q => _rng.Next()).ToList();

            PlayQuestions = new ObservableCollection<Question>(shuffled);

            foreach (var q in PlayQuestions)
            {
                var allAnswers = new List<string>(q.IncorrectAnswers) { q.CorrectAnswer };
                q.ShuffledAnswers = allAnswers.OrderBy(a => _rng.Next()).ToList();
            }

            CurrentQuestionIndex = 0;
            CurrentQuestion = PlayQuestions[CurrentQuestionIndex];

            StartTimer(ActivePack.TimeLimitInSeconds);
        }

        private async Task ShowFeedbackIndicators(int? selectedIndex = null)
        {
            if (CurrentQuestion == null)
                return;

            for (int i = 0; i < 4; i++)
            {
                ShowCorrectIndicator[i] = false;
                ShowIncorrectIndicator[i] = false;
            }


            int correctIndex = CurrentQuestion.ShuffledAnswers.IndexOf(CurrentQuestion.CorrectAnswer);


            ShowCorrectIndicator[correctIndex] = true;


            if (selectedIndex.HasValue && selectedIndex.Value != correctIndex)
            {
                ShowIncorrectIndicator[selectedIndex.Value] = true;
            }


            RaisePropertyChanged(nameof(ShowCorrectIndicator));
            RaisePropertyChanged(nameof(ShowIncorrectIndicator));


            await Task.Delay(1800);


            for (int i = 0; i < 4; i++)
            {
                ShowCorrectIndicator[i] = false;
                ShowIncorrectIndicator[i] = false;
            }
            RaisePropertyChanged(nameof(ShowCorrectIndicator));
            RaisePropertyChanged(nameof(ShowIncorrectIndicator));
        }

    }
}
