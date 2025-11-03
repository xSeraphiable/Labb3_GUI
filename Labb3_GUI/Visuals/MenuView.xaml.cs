using Labb3_GUI.Dialogs;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Labb3_GUI.Visuals
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
        }



        private void newQuestionPack_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateNewPackDialog();

            dialog.ShowDialog();

        }

        private void EditPack_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PackOptionsDialog();

            dialog.ShowDialog();
        }
    }

}
