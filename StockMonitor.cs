using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StockMonitor
{
    public class StockMonitor : IDisposable
    {
        private readonly StockTicker _ticker;
        Dictionary<string, StockInfo> _stockInfos = new Dictionary<string, StockInfo>();

        public StockMonitor(StockTicker ticker)
        {
            _ticker = ticker;
            ticker.StockTick += OnStockTick;
        }

        public StockMonitor()
        {
            throw new NotImplementedException();
        }

        void OnStockTick(object sender, StockTick stockTick)
        {
            const decimal maxChangeRatio = 0.1m;

            var ticks = Observable.FromEventPattern<EventHandler<StockTick>, StockTick>(
                h => ticker.StockTick += h,
                h => ticker.StockTick -= h)
                .Select(tickEvent => tickEvent.EventArgs)
                .Synchronize();
            )

            StockInfo stockInfo;
            var quoteSymbol = stockTick.QuoteSymbol;
            var stockInfoExists = _stockInfos.TryGetValue(quoteSymbol, out stockInfo);
            if (stockInfoExists)
            {
                var priceDiff = stockTick.Price - stockInfo.PrevPrice;
                var changeRatio = Math.Abs(priceDiff / stockInfo.PrevPrice);
                if (changeRatio > maxChangeRatio)
                {
                    Debug.WriteLine("Stock:{0} has changed with {1} ratio - OldPrice:{2} NewPrice:{3}",
                        quoteSymbol, changeRatio, stockInfo.PrevPrice, stockTick.Price);
                }
                _stockInfos[quoteSymbol].PrevPrice = stockTick.Price;
            }
            else
            {
                _stockInfos[quoteSymbol] = new StockInfo(quoteSymbol, stockTick.Price);
            }
        }

        public void Dispose()
        {
            _ticker.StockTick -= OnStockTick;
            _stockInfos.Clear();
        }
    }
}