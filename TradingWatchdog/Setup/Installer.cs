using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using MT5Wrapper;
using MT5Wrapper.Interface;
using TradingWatchdog.Logging;
using TradingWatchdog.Monitor;
using TradingWatchdog.ServerListener;
using TradingWatchdog.Settings;
using TradingWatchdog.Watchdog;

namespace TradingWatchdog.Setup
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<TypedFactoryFacility>();

            container.Register(
                Classes
                    .FromThisAssembly()
                    .BasedOn(typeof(ISettingsRepository<>))
                    .WithServiceAllInterfaces()
                    .LifestyleTransient()
            );

            container.Register(
                Component
                    .For<IMT5Api>()
                    .ImplementedBy<MT5Api>()
                    .LifestyleTransient()
            );

            container.Register(
                Component
                    .For<ITradingWatchdog>()
                    .ImplementedBy<Watchdog.TradingWatchdog>()
                    .LifestyleTransient()
            );
            
            container.Register(
                Component
                    .For<ITradesMonitor>()
                    .ImplementedBy<TradesMonitor>()
                    .LifestyleSingleton()
            );

            container.Register(
                Component
                    .For<ITradeValidator>()
                    .ImplementedBy<TradeValidator>()
                    .LifestyleSingleton()
            );

            container.Register(
                Component
                    .For<ITradingServerListener>()
                    .ImplementedBy<TradingServerListener>()
                    .LifestyleTransient()
            );
            
            container.Register(
                Component
                    .For<ITradingServerListenerFactory>()
                    .AsFactory()
            );

            container.Register(
                Component
                    .For<ILogger>()
                    .ImplementedBy<FileLogger>()
                    .LifestyleSingleton()
            );

            container.Register(
                Component
                    .For<ILogEntryFormatter>()
                    .ImplementedBy<JsonLogEntryFormatter>()
                    .LifestyleSingleton()
            );
        }
    }
}
