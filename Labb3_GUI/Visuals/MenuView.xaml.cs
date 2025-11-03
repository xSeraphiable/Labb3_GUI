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

            if (dialog.ShowDialog() == true)
            {
                //TODO: lägg till difficulty (först i createpackdialog och sedan här)
                var model = new QuestionPack(dialog.PackName, timeLimitInSeconds: dialog.TimeLimit);

                var newPack = new QuestionPackViewModel(model);

                
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.Packs.Add(newPack);

                    vm.ActivePack = newPack;
                }
            }


        }

        private void EditPack_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PackOptionsDialog();

            dialog.ShowDialog();
        }
    }

}
