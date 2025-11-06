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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Labb3_GUI.Dialogs
{
    /// <summary>
    /// Interaction logic for CreateNewPackDialog.xaml
    /// </summary>
    public partial class CreateNewPackDialog : Window
    {
        public CreateNewPackDialog()
        {
            InitializeComponent();
            DataContext = ((MainWindow)App.Current.MainWindow).DataContext;
            (DataContext as MainWindowViewModel).CloseDialog = () => this.Close();
        }

        
        //private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        //{
        //    DialogResult = true;
        //    Close();
        //}

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
