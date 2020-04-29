using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradingWatchdog.Logging;
using TradingWatchdog.Monitor;
using TradingWatchdog.Settings;
using static MetaQuotes.MT5CommonAPI.CIMTDeal;

namespace TradingWatchdog.Tests
{
    [TestClass]
    public class TradesMonitor_Tests
    {
        [TestMethod]
        public void WhenOneTradeIsMatched()
        {
            // arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var underTest = GetObjectUnderTest();
            var trades = GetTradesForFirstScenario();

            // act
            var task = Task.Run(() => underTest.Monitor(cancellationToken));
            trades.ForEach(t => underTest.QueuedTrades.Enqueue(t));
            cancellationTokenSource.CancelAfter(1000);
            Task.WaitAll(task);

            // assert
            Assert.IsTrue(underTest.QueuedTrades.Count == 0, "All trades should be processed");
            Assert.IsTrue(underTest.ProcessedTrades.Count == 3, "All trades should be processed");
            Assert.IsTrue(underTest.FlaggedTrades.Count == 1, "Third trade should trigger match with the second one");
            Assert.IsTrue(underTest.FlaggedTrades[0].Trade.Id == trades[2].Id);
            Assert.IsTrue(underTest.FlaggedTrades[0].Match.Id == trades[1].Id);
        }

        [TestMethod]
        public void WhenMultipleTradesAreMatched()
        {
            // arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var trades = GetTradesForSecondScenario();
            var underTest = GetObjectUnderTest();

            // act
            var task = Task.Run(() => underTest.Monitor(cancellationToken));
            trades.ForEach(t => underTest.QueuedTrades.Enqueue(t));
            cancellationTokenSource.CancelAfter(1000);
            Task.WaitAll(task);

            // assert
            Assert.IsTrue(underTest.QueuedTrades.Count == 0, "All trades should be processed");
            Assert.IsTrue(underTest.ProcessedTrades.Count == 5, "All trades should be processed");
            Assert.IsTrue(underTest.FlaggedTrades.Count == 3, "Fourth trade triggers one match and fifth matches two of them");
            Assert.IsTrue(underTest.FlaggedTrades[0].Trade.Id == trades[3].Id);
            Assert.IsTrue(underTest.FlaggedTrades[0].Match.Id == trades[1].Id);
            Assert.IsTrue(underTest.FlaggedTrades[1].Trade.Id == trades[4].Id);
            Assert.IsTrue(underTest.FlaggedTrades[1].Match.Id == trades[1].Id);
            Assert.IsTrue(underTest.FlaggedTrades[2].Trade.Id == trades[4].Id);
            Assert.IsTrue(underTest.FlaggedTrades[2].Match.Id == trades[3].Id);
        }

        private static TradesMonitor GetObjectUnderTest()
        {
            var validationSettings = new ValidationSettings()
            {
                OpenTimeDelta = 1,
                VolumeToBalanceRatio = 0.05m,
            };
            var monitoringSettings = new MonitoringSettings()
            {
                RetentionPeriod = 30
            };
            var settingsRepositoryMock = new Mock<IValidationSettingsRepository>();
            settingsRepositoryMock.Setup(r => r.Get()).Returns(validationSettings);
            var monitoringSettingsRepositoryMock = new Mock<IMonitoringSettingsRepository>();
            monitoringSettingsRepositoryMock.Setup(r => r.Get()).Returns(monitoringSettings);
            var validator = new TradeValidator(settingsRepositoryMock.Object);
            var loggerMock = new Mock<ILogger>();
            var underTest = new TradesMonitor(validator, loggerMock.Object, monitoringSettingsRepositoryMock.Object);
            return underTest;
        }

        private static List<Trade> GetTradesForFirstScenario()
        {
            return new List<Trade>()
            {
                new Trade()
                {
                    Id = 1,
                    Balance = 10000,
                    Action = EnDealAction.DEAL_BUY,
                    Symbol = "EURUSD",
                    Volume = 1000,
                    AsAt = new DateTime(2019, 5, 12, 14, 43, 12),
                    Description = "Deal #1, Balance 10 000, Buy EURUSD 1 lot at 2019-05-12 14:43:12",
                    Login = 1,
                    Server = "Mock 1"
                },
                new Trade()
                {
                    Id = 2,
                    Balance = 10000,
                    Action = EnDealAction.DEAL_SELL,
                    Symbol = "GBPUSD",
                    Volume = 200,
                    AsAt = new DateTime(2019, 5, 12, 14, 43, 23),
                    Description = "Deal #2, Balance 10 000, Sell GBPUSD 0.2 lots at 2019-05-12 14:43:23",
                    Login = 2,
                    Server = "Mock 2"
                },
                new Trade()
                {
                    Id = 3,
                    Balance = 10000,
                    Action = EnDealAction.DEAL_SELL,
                    Symbol = "GBPUSD",
                    Volume = 210,
                    AsAt = new DateTime(2019, 5, 12, 14, 43, 24),
                    Description = "Deal #3, Balance 10 000, Sell GBPUSD 0.21 lot at 2019-05-12 14:43:24",
                    Login = 3,
                    Server = "Mock 1"
                },
            };
        }

        private static List<Trade> GetTradesForSecondScenario()
        {
            return new List<Trade>()
            {
                new Trade()
                {
                    Id = 1,
                    Balance = 10000,
                    Action = EnDealAction.DEAL_BUY,
                    Symbol = "EURUSD",
                    Volume = 1000,
                    AsAt = new DateTime(2019, 5, 12, 14, 43, 12),
                    Description = "Deal #1, Balance 10 000, Buy EURUSD 1 lot at 2019-05-12 14:43:12",
                    Login = 1,
                    Server = "Mock 1"
                },
                new Trade()
                {
                    Id = 2,
                    Balance = 10000,
                    Action = EnDealAction.DEAL_SELL,
                    Symbol = "GBPUSD",
                    Volume = 200,
                    AsAt = new DateTime(2019, 5, 12, 14, 43, 23),
                    Description = "Deal #2, Balance 10 000, Sell GBPUSD 0.2 lots at 2019-05-12 14:43:23",
                    Login = 2,
                    Server = "Mock 2"
                },

                new Trade()
                {
                    Id = 3,
                    Balance = 1000,
                    Action = EnDealAction.DEAL_SELL,
                    Symbol = "GBPUSD",
                    Volume = 1200,
                    AsAt = new DateTime(2019, 5, 12, 14, 43, 23),
                    Description = "Deal #3, Balance 1 000, Sell GBPUSD 1.2 lots at 2019-05-12 14:43:23",
                    Login = 2,
                    Server = "Mock 1"
                },
                new Trade()
                {
                    Id = 4,
                    Balance = 10000,
                    Action = EnDealAction.DEAL_SELL,
                    Symbol = "GBPUSD",
                    Volume = 210,
                    AsAt = new DateTime(2019, 5, 12, 14, 43, 24),
                    Description = "Deal #4, Balance 10 000, Sell GBPUSD 0.21 lot at 2019-05-12 14:43:24",
                    Login = 3,
                    Server = "Mock 1"
                },
                new Trade()
                {
                    Id = 5,
                    Balance = 20000,
                    Action = EnDealAction.DEAL_SELL,
                    Symbol = "GBPUSD",
                    Volume = 400,
                    AsAt = new DateTime(2019, 5, 12, 14, 43, 24),
                    Description = "Deal #5, Balance 20 000, Sell GBPUSD 0.4 lot at 2019-05-12 14:43:24",
                    Login = 1,
                    Server = "Mock 2"
                },
            };
        }
    }
}
