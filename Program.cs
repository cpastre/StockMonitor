using System;

namespace StockMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            StockTicker ticker = new StockTicker();
            StockMonitor sm = new StockMonitor(ticker);
            ticker.Tick(new StockTick{ QuoteSymbol = "MSFT", Price = 100 });
            ticker.Tick(new StockTick{ QuoteSymbol = "INTC", Price = 150 });
            ticker.Tick(new StockTick{ QuoteSymbol = "MSFT", Price = 170 });
            ticker.Tick(new StockTick{ QuoteSymbol = "MSFT", Price = 195 });
            
        }
    }
}
