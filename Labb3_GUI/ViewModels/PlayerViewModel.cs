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



        private int _timeLeft;

        public int TimeLeft
        {
            get { return _timeLeft; }
            set { _timeLeft = value; RaisePropertyChanged(); }
        }

        


        private bool _isAnswerCorrect;
        public bool IsAnswerCorrect
        {
            get => _isAnswerCorrect;
            set { _isAnswerCorrect = value; RaisePropertyChanged(); }
        }

        private bool _showFeedback;
        public bool ShowFeedback
        {
            get => _showFeedback;
            set { _showFeedback = value; RaisePropertyChanged(); }
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

            var selectedAnswer = args as string;
            if (selectedAnswer == null) return;

            if (CurrentQuestion == null)
                return;

            IsAnswerCorrect = selectedAnswer == CurrentQuestion.CorrectAnswer;
            
            if (IsAnswerCorrect) {
                MessageBox.Show("fungerar");
                Score++;
                                   }

            _timer.Stop();

            // Visa feedback (t.ex. via boolar för färg)
            ShowFeedback = true;

            await Task.Delay(1500);

            ShowFeedback = false;
            MoveToNextQuestion();
        }

        private async void OnTimeUp()
        {

            _timer.Stop();

            // Visa feedback (t.ex. via boolar för färg)
            ShowFeedback = true;

            await Task.Delay(1500);

            ShowFeedback = false;
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


    }
}
