using System.Configuration;
using System.Windows;
using EasyNetQ;
using Messages;

namespace Database.Backup.Client
{
    public partial class MainWindow
    {
        private readonly IBus bus;

        public MainWindow()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["easynetq"].ConnectionString;
            bus = RabbitHutch.CreateBus(connectionString);

            InitializeComponent();
        }

        private async void MakeBackup(object sender, RoutedEventArgs e)
        {
            Backup.IsEnabled = false;
            BusyIndicator.Visibility = Visibility.Visible;

            var response = await bus.RequestAsync<BackupDatabaseRequest, BackupDatabaseResponse>(new BackupDatabaseRequest());

            string message = response.Success ? "success" : "failed";
            MessageBox.Show($"Creating backup took {response.DatabaseBackupTime.TotalSeconds:#.###} seconds.", message);

            Backup.IsEnabled = true;
            BusyIndicator.Visibility = Visibility.Collapsed;
        }
    }
}
