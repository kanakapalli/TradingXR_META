using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class apicallcehck : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] TextMeshProUGUI text;
    void Start()
    {

        StartCoroutine(StockApiService.Instance.SearchStocks("z", OnDataReceived, OnError));
        User sampleUser = new User
        {
            Username = "JohnDoe",
            Email = "johndoe@example.com",
            Office = "None",
            Age = 30,
            Bio = "Just a sample user for testing.",
            LoginCode = "123456",
            ConversionThread = null,
            ProfilePicture = null
        };
        UserManager.SaveUserData(sampleUser);
    }

    private void OnDataReceived(List<StockData> stockData)
    {
        User user = UserManager.LoadUserData();
        text.text = $"Username: {user.Username} - Bio: {user.Bio}";
        foreach (var stock in stockData)
        {

            Debug.Log($"Stock: {stock.Symbol} - Last Sale: {stock.Last_Sale}");
        }
    }

    private void OnError(string error)
    {
        Debug.LogError("Failed to get stock data: " + error);
    
}
}
