using Castle.Windsor;
using Castle.Windsor.Installer;
using System;
using System.Threading;
using System.Threading.Tasks;
using TradingWatchdog.Watchdog;

namespace TradingWatchdog
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var container = new WindsorContainer();
            container.Install(FromAssembly.This());

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var tasks = container
                .Resolve<ITradingWatchdog>()
                .Start(cancellationToken);

            Console.WriteLine("Press enter to stop the watchdog");
            Console.WriteLine();
            Console.ReadLine();
            Console.WriteLine();

            cancellationTokenSource.Cancel();

            await Task.WhenAll(tasks);

            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
