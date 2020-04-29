using MetaQuotes.MT5CommonAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using TradingWatchdog.Monitor;
using TradingWatchdog.Settings;

namespace TradingWatchdog.Tests
{
    [TestClass]
    public class TradeValidator_Tests
    {
        public ValidationSettings Settings { get; } = new ValidationSettings()
        {
            OpenTimeDelta = 1,
            VolumeToBalanceRatio = 0.05m
        };

        [TestMethod]
        public void WhenItIsTheSame_TradeIsFlagged()
        {
            // arrange
            var underTest = GetObjectUnderTest();
            var trade = GetDefaultTrade();

            // act
            var isSuspicious = underTest.IsSuspicious(trade, trade);

            // assert
            Assert.IsTrue(isSuspicious, "Same trade should be flagged");
        }

        [TestMethod]
        public void WhenInsideOfOpenTimeDelta_TradeIsFlagged()
        {
            // arrange
            var underTest = GetObjectUnderTest();

            // act
            var trade = GetDefaultTrade();
            var trade2 = GetDefaultTrade();
            trade2.AsAt = trade.AsAt.AddSeconds(Settings.OpenTimeDelta);
            var isSuspicious = underTest.IsSuspicious(trade, trade2);

            // assert
            Assert.IsTrue(isSuspicious, "Trade within the open time delata period should be flagged");
        }

        [TestMethod]
        public void WhenVolumeToBalanceRatioIsLowerThanGivenPercentage_TradeIsFlagged()
        {
            // arrange
            var underTest = GetObjectUnderTest();

            // act
            var trade = GetDefaultTrade();
            var trade2 = GetDefaultTrade();
            trade2.Balance = 20000;
            var isSuspicious = underTest.IsSuspicious(trade, trade2);

            // assert
            Assert.IsTrue(isSuspicious, "Trade within the open time delata period should be flagged");
        }

        [TestMethod]
        public void WhenCurrencyPairDifferes_TradeIsNotFlagged()
        {
            // arrange
            var underTest = GetObjectUnderTest();

            // act
            var trade = GetDefaultTrade();
            var trade2 = GetDefaultTrade();
            trade2.Symbol = "EURUSD";
            var isSuspicious = underTest.IsSuspicious(trade, trade2);

            // assert
            Assert.IsFalse(isSuspicious, "Different currency symbols should not be flagged");
        }

        [TestMethod]
        public void WhenActionIsNotBuyOrSell_TradeIsNotFlagged()
        {
            // arrange
            var underTest = GetObjectUnderTest();

            // act
            var trade = GetDefaultTrade();
            var trade2 = GetDefaultTrade();
            trade2.Action = CIMTDeal.EnDealAction.DEAL_CHARGE;
            var isSuspicious = underTest.IsSuspicious(trade, trade2);

            // assert
            Assert.IsFalse(isSuspicious, "Actions apart from buy/sell should not be flagged");
        }

        [TestMethod]
        public void WhenOutsideOfOpenTimeDelta_TradeIsNotFlagged()
        {
            // arrange
            var underTest = GetObjectUnderTest();

            // act
            var trade = GetDefaultTrade();
            var trade2 = GetDefaultTrade();
            trade2.AsAt = DateTime.Now.AddSeconds(Settings.OpenTimeDelta * 2);
            var isSuspicious = underTest.IsSuspicious(trade, trade2);

            // assert
            Assert.IsFalse(isSuspicious, "When trade falls outside of open time delta it should not be flagged");
        }

        [TestMethod]
        public void WhenVolumeToBalanceRatioIsBiggerThanGivenPercentage_TradeIsNotFlagged()
        {
            // arrange
            var underTest = GetObjectUnderTest();

            // act
            var trade = GetDefaultTrade();
            var trade2 = GetDefaultTrade();
            trade2.Balance = 20001;
            var isSuspicious = underTest.IsSuspicious(trade, trade2);

            // assert
            Assert.IsFalse(isSuspicious, "When volume to balance ratio is more than given percentage it should not be flagged");
        }

        private static Trade GetDefaultTrade()
        {
            return new Trade()
            {
                Symbol = "GBPUSD",
                Action = CIMTDeal.EnDealAction.DEAL_SELL,
                Balance = 10000,
                AsAt = DateTime.Now,
                Volume = 1000,
            };
        }

        private TradeValidator GetObjectUnderTest()
        {
            var settingsRepositoryMock = new Mock<IValidationSettingsRepository>();
            settingsRepositoryMock.Setup(s => s.Get()).Returns(Settings);
            var underTest = new TradeValidator(settingsRepositoryMock.Object);
            return underTest;
        }
    }
}
