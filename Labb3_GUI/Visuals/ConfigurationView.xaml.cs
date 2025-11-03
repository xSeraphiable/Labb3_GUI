using Labb3_GUI.Dialogs;
using Labb3_GUI.Models;
using Labb3_GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Labb3_GUI.Visuals
{
    /// <summary>
    /// Interaction logic for ConfigurationView.xaml
    /// </summary>
    public partial class ConfigurationView : System.Windows.Controls.UserControl
    {
        private int count = 1;
        public ConfigurationView()
        {
            InitializeComponent();

            
        }



        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PackOptionsDialog();

            if (dialog.ShowDialog() == true) { }

        }        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            QuestionPackViewModel viewModel = (DataContext as QuestionPackViewModel);
            viewModel.Name = "New name";
            viewModel.Questions.Add(new Question($"Fråga {count++}", "2", "3", "1", "4"));

        }
    }
}
