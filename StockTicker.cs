using System;

namespace StockMonitor
{
    public class StockTicker
    {
        public event EventHandler<StockTick> StockTick;

        public void Tick(StockTick tick)
        {
            StockTick(this, tick);
        }
    }
}