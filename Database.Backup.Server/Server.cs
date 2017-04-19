using System;
using System.Configuration;
using System.Diagnostics;
using EasyNetQ;
using log4net;
using Messages;

namespace Database.Backup.Server
{
    public class Server
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string databaseName = ConfigurationManager.AppSettings["databaseName"];
        private readonly string backupFolder = ConfigurationManager.AppSettings["backupFolder"];
        private readonly string serverName = ConfigurationManager.AppSettings["serverName"];
        private readonly string credentials = ConfigurationManager.AppSettings["credentials"];
        private readonly bool zipBackup = Convert.ToBoolean(ConfigurationManager.AppSettings["zipBackup"]);

        public void Start()
        {
            var bus = RabbitHutch.CreateBus(ConfigurationManager.ConnectionStrings["easynetq"].ConnectionString);
            bus.Respond<BackupDatabaseRequest, BackupDatabaseResponse>(HandleDatabaseBackupRequest);
        }

        private BackupDatabaseResponse HandleDatabaseBackupRequest(BackupDatabaseRequest request)
        {
            var stopWatch = Stopwatch.StartNew();

            try
            {
                Log.Info("Creating database backup");

                CreateBackup();
                stopWatch.Stop();

                Log.Info($"Succesfully created a database backup in: {stopWatch.ElapsedMilliseconds}ms.");
                return new BackupDatabaseResponse { Success = true, DatabaseBackupTime = stopWatch.Elapsed };
            }
            catch (Exception e)
            {
                Log.Error("Creating database backup failed.", e);
                return new BackupDatabaseResponse { DatabaseBackupTime = stopWatch.Elapsed };
            }
        }

        private void CreateBackup()
        {
            DatabaseMinder.Handler.HandleCommand(new DatabaseMinder.CommandArgs
            {
                Backup = true,
                ServerName = serverName,
                DatabaseName = databaseName,
                NameOfCredentials = credentials,
                Folder = backupFolder,
                ZipBackup = zipBackup
            });
        }
    }
}