using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace HMI_CoolingEquipment.Views.Dialog
{
    /// <summary>
    /// Interaction logic for TaskRunningDialog.xaml
    /// </summary>
    public partial class TaskRunningDialog : Window, INotifyPropertyChanged
    {
        private string taskRunningContent;
        public string TaskRunningContent
        {
            get { return taskRunningContent; }
            set
            {
                taskRunningContent = value;
                OnPropertyChanged(nameof(TaskRunningContent));
            }
        }

        public TaskRunningDialog(string taskName)
        {
            InitializeComponent();
            this.DataContext = this;
            TaskRunningContent = $"{taskName} in progress...";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public void CloseDialog()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DialogResult = true;
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Notify Property Change
        public event PropertyChangedEventHandler PropertyChanged;

        // The method to raise the PropertyChanged event
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
