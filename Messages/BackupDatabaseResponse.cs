using System;

namespace Messages
{
    public class BackupDatabaseResponse
    {
        public bool Success { get; set; }
        public TimeSpan DatabaseBackupTime { get; set; }
    }
}