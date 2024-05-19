using System.Collections.Generic;

public class StockDataResponse
{
    public List<StockData> Data { get; set; }
}

public class StockData
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public string Last_Sale { get; set; }
    public string Net_Change { get; set; }
    public string Percent_Change { get; set; }
    public string Market_Cap { get; set; }
    public string Country { get; set; }
    public string Ipo_Year { get; set; }
    public string Volume { get; set; }
    public string Sector { get; set; }
    public string Industry { get; set; }
}

public struct StockStruct
{
    public string Name;
    public string Symbol;
    public string Price;
    public string Percent;
    public string Overview_URL;
    public string Detail_URL;
}