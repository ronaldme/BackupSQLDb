using System.Diagnostics;
using Topshelf;

namespace Database.Backup.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<Server>(s =>
                {
                    s.ConstructUsing(() => new Server());
                    s.WhenStarted(l => l.Start());
                    s.WhenStopped(l => { }); // required
                });

                x.RunAsLocalService();
                x.SetServiceName("Database.Backup.Server");

                x.DependsOn("RabbitMQ");
                x.DependsOn("LanmanServer");

                x.StartAutomatically();
                x.UseLog4Net("log4net.config");
            });

            if (exitCode != TopshelfExitCode.Ok && Debugger.IsAttached) Debugger.Break();
        }
    }
}
