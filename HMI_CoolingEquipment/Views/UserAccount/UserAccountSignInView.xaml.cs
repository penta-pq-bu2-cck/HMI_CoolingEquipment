using HMI_CoolingEquipment.Models;
using HMI_CoolingEquipment.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HMI_CoolingEquipment.Views
{
    /// <summary>
    /// Interaction logic for UserAccountSignInView.xaml
    /// </summary>
    public partial class UserAccountSignInView : Window
    {
        private DateTime startTypeIn {  get; set; }
        public UserAccountSignIn_VM ViewModel
        {
            get;
        }

        public UserAccountSignInView(UserAccountSignIn_VM viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
            IsVisibleChanged += UserAccountSignInView_IsVisibleChanged;
        }

        private void UserAccountSignInView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UserAccountUserNameTextBox.Clear();
            UserAccountPasswordBox.Clear();
            SignInBtn.IsDefault = false;

            UserAccountUserNameTextBox.Focus();
        }

        private void UserAccountUserNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DateTime.Now - startTypeIn <= TimeSpan.FromSeconds(double.Parse(ConfigurationManager.AppSettings["BarcodeLoginTime"])))
                {
                    if (ViewModel.UserSignIn(UserAccountUserNameTextBox.Text.Trim()))
                    {
                        UserAccountSignInWindow.Close();
                    }
                    else
                    {
                        LogHelper.General(3, $"Username = [{UserAccountUserNameTextBox.Text}] not exist");
                        UserAccountSignInMessageTextBlock.Visibility = Visibility.Visible;
                        UserAccountUserNameTextBox.Clear();
                        UserAccountPasswordBox.Clear();
                        UserAccountUserNameTextBox.Focus();
                        SignInBtn.IsDefault = false;
                    }
                }
                else
                {
                    UserAccountPasswordBox.Focus();
                }
            }
        }

        private void UserAccountPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SignInBtn.IsDefault = true;
            }
        }

        private void UsernameTextChanged(object sender, TextChangedEventArgs e)
        {
            if(UserAccountUserNameTextBox.Text.Length == 1)
            {
                startTypeIn = DateTime.Now;
            }
        }
    }
}
