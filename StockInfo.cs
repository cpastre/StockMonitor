namespace StockMonitor
{
    class StockInfo
    {
        public string Symbol { get; set; }
        public decimal PrevPrice { get; set; }

        public StockInfo(string symbol, decimal price)
        {
            Symbol = symbol;
            PrevPrice = price;        
        }
    }
}