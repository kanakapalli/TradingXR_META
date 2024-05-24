using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class JsonDatahandler
{
    public static JsonData HandleJsonData(string jsonData)
    {
        JObject jsonObject = JObject.Parse(jsonData);
        List<StockData> m_stock_data_list = new List<StockData>();
        // Extract count, next, and previous
        int count = (int)jsonObject["data"]["count"];
        string next = (string)jsonObject["data"]["next"];
        string previous = (string)jsonObject["data"]["previous"];

        Debug.Log($"Count: {count}");
        Debug.Log($"Next: {next}");
        Debug.Log($"Previous: {previous}");

        // Extract and print stock data
        JArray results = (JArray)jsonObject["data"]["results"];
        foreach (JToken result in results)
        {
            string symbol = (string)result["symbol"];
            string name = (string)result["name"];
            string lastSale = (string)result["last_sale"];
            string netChange = (string)result["net_change"];
            string percentChange = (string)result["percent_change"];
            string marketCap = (string)result["market_cap"];
            string country = (string)result["country"];
            string ipoYear = (string)result["ipo_year"];
            string volume = (string)result["volume"];
            string sector = (string)result["sector"];
            string industry = (string)result["industry"];

            m_stock_data_list.Add(
                    new StockData()
                    {
                        Name = (string)result["name"],
                        Symbol = (string)result["symbol"],
                        Last_Sale = (string)result["last_sale"],
                        Percent_Change = (string)result["percent_change"]
                    }
                );

            Debug.Log($"Symbol: {symbol}, Name: {name}, Last Sale: {lastSale}, Net Change: {netChange}, Percent Change: {percentChange}, Market Cap: {marketCap}, Country: {country}, IPO Year: {ipoYear}, Volume: {volume}, Sector: {sector}, Industry: {industry}");
        }
        
        return new JsonData { m_Count = count, m_Next = next, m_Previous = previous, m_Results = m_stock_data_list};
    }
}

public struct JsonData
{
    public int m_Count;
    public string m_Next;
    public string m_Previous;

    public List<StockData> m_Results;
}