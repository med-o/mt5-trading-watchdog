using MetaQuotes.MT5CommonAPI;
using System;
using System.Collections.Generic;
using TradingWatchdog.Settings;

namespace TradingWatchdog.Monitor
{
    public class TradeValidator : ITradeValidator
    {
        private readonly IValidationSettingsRepository _settings;

        public TradeValidator(IValidationSettingsRepository settings)
        {
            _settings = settings;
        }

        public bool IsSuspicious(Trade x, Trade y)
        {
            return HasSameCurrencyPair(x, y)
                && IsBuyOrSellAction(x, y)
                && AreLessThenOpenTimeDeltaSecondsApart(x, y)
                && VolumeToBalanceRatioIsNoMoreThanGivenPercentage(x, y);
        }

        private static bool IsBuyOrSellAction(Trade x, Trade y)
        {
            var allowedActions = new List<CIMTDeal.EnDealAction>()
            {
                CIMTDeal.EnDealAction.DEAL_BUY,
                CIMTDeal.EnDealAction.DEAL_FIRST,
                CIMTDeal.EnDealAction.DEAL_SELL
            };

            return allowedActions.Contains(x.Action) && allowedActions.Contains(y.Action);
        }

        private bool VolumeToBalanceRatioIsNoMoreThanGivenPercentage(Trade x, Trade y)
        {
            var percentage = _settings.Get().VolumeToBalanceRatio;
            return Math.Abs(x.VolumeToBalanceRatio - y.VolumeToBalanceRatio) <= percentage;
        }

        private static bool HasSameCurrencyPair(Trade x, Trade y)
        {
            return x.Symbol == y.Symbol;
        }

        private bool AreLessThenOpenTimeDeltaSecondsApart(Trade x, Trade y)
        {
            var delta = _settings.Get().OpenTimeDelta;
            return Math.Abs(x.AsAt.Subtract(y.AsAt).TotalSeconds) <= delta;
        }
    }
}
