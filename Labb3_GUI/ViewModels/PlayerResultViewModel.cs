using Labb3_GUI.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3_GUI.ViewModels
{
    internal class PlayerResultViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public int Score { get; }
        public int TotalQuestions { get; }

        public string ScoreText => $"You scored {Score} out of {TotalQuestions}!";

        public DelegateCommand RestartQuizCommand { get; }

        public PlayerResultViewModel(MainWindowViewModel mainWindowViewModel, int score, int totalQuestions)
        {
            _mainWindowViewModel = mainWindowViewModel;
            Score = score;
            TotalQuestions = totalQuestions;

            RestartQuizCommand = new DelegateCommand(RestartQuiz);
        }

        private void RestartQuiz(object? args)
        {
            var playerVm = _mainWindowViewModel.PlayerViewModel;
            playerVm.InitializeQuiz();
            _mainWindowViewModel.CurrentView = playerVm;
        }
    }
}
