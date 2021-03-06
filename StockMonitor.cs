using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;

namespace StockMonitor
{
    public class StockMonitor : IDisposable
    {
        private readonly StockTicker _ticker;
        Dictionary<string, StockInfo> _stockInfos = new Dictionary<string, StockInfo>();
        private IDisposable _subscription;
    
        public StockMonitor(StockTicker ticker)
        {
            const decimal maxChangeRatio = 0.1m;

            var ticks = Observable.FromEventPattern<EventHandler<StockTick>, StockTick>(
                h => ticker.StockTick += h,
                h => ticker.StockTick -= h)
                .Select(tickEvent => tickEvent.EventArgs)
                .Synchronize();

            var drasticChanges =
                from tick in ticks
                group tick by tick.QuoteSymbol
                into company
                from tickPair in company.Buffer(2,1)
                let changeRatio = Math.Abs((tickPair[1].Price - tickPair[0].Price)/tickPair[0].Price)
                where changeRatio > maxChangeRatio
                select new 
                {
                    Symbol = company.Key,
                    ChangeRatio = changeRatio,
                    OldPrice = tickPair[0].Price,
                    NewPrice = tickPair[1].Price
                };
            
            _subscription = drasticChanges.Subscribe( change =>
            {
                Console.WriteLine($"Stock: {change.Symbol} has changed with {change.ChangeRatio} ratio, Old Price: {change.OldPrice} New Price: {change.NewPrice}");
            },
            ex => {},
            () => {}
            );
        }

        public StockMonitor()
        {
            throw new NotImplementedException();
        }

        void OnStockTick(object sender, StockTick stockTick)
        {
            const decimal maxChangeRatio = 0.1m;

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
            _subscription.Dispose();
        }
    }
}