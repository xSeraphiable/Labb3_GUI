using Labb3_GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;


namespace Labb3_GUI.Services
{
    internal class AppNavigator
    {
        public event Action<object> ViewChanged;
        public event Action NavigatingAwayFromQuiz;

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                ViewChanged?.Invoke(value);
            }
        }

        public void NavigateTo(object newView)
        {
            CurrentView = newView;
        }
    }
}
