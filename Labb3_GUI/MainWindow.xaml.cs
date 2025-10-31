using Labb3_GUI.Dialogs;
using Labb3_GUI.Models;
using Labb3_GUI.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Labb3_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var pack = new QuestionPack("MyQuestionPack");
            DataContext = new QuestionPackViewModel(pack);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PackOptionsDialog();

            // Visa som modalt fönster (användaren måste stänga det innan de går vidare)
            dialog.ShowDialog();

            // ...eller icke-modalt (öppet parallellt):
            // dialog.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //KOLLA UPP ANTECKNINGAR FRÅN LEKTIONEN!
            //var myviewModel = new QuestionPackViewModel();
            //(DataContext as QuestionPackViewModel).Name = "New name";
            
        }
    }
}