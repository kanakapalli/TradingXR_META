using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oculus.Interaction;
using UnityEngine;

public class LLM_Model
{
    // Define the endpoint URL
    private string url = "http://13.235.244.64:8000/api/txr-bot/";

    public static LLM_Model Instance { get; private set; } = new LLM_Model();

    private LLM_Model() { }

    public async Task<LLM> SendRequestAsync(string prompt)
    {
        // Define the request body
        var requestBody = new
        {
            prompt = prompt
        };

        // Convert the request body to JSON
        string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

        // Create an HTTP client
        using (HttpClient client = new HttpClient())
        {
            // Create the content for the POST request
            StringContent content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            // Send the POST request to the API endpoint
            HttpResponseMessage response = await client.PostAsync(url, content);

            // Ensure the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as a string
                string responseContent = await response.Content.ReadAsStringAsync();

                // Parse the response content as a JObject
                JObject jsonResponse = JObject.Parse(responseContent);

                // Extract the desired values from the JObject
                string reply = jsonResponse["reply"].ToString();
                JArray chatHistory = (JArray)jsonResponse["chat_history"];
                string chatId = jsonResponse["chat_id"].ToString();

                // Print the extracted values using Debug.Log
                Debug.Log("Reply: " + reply);
                Debug.Log("Chat History: " + chatHistory.ToString());
                Debug.Log("Chat ID: " + chatId);

                LLM m_LLM_Response = new LLM
                {
                    result = reply,
                    history = chatHistory
                };
                return m_LLM_Response;
            }
            else
            {
                // If the request was not successful, print the status code and reason
                Debug.Log("Request failed with status code: " + response.StatusCode);
                Debug.Log("Reason: " + response.ReasonPhrase);

                string m_result = "";

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        Debug.Log("Success");
                        m_result = response.StatusCode + ": Status Ok";
                        break;
                    case System.Net.HttpStatusCode.MovedPermanently:
                        Debug.Log("Permanent Redirect");
                        m_result = response.StatusCode + ": MovedPermanently";
                        break;
                    case System.Net.HttpStatusCode.Found:
                        Debug.Log("Temporary Redirect");
                        m_result = response.StatusCode + ": Found";
                        break;
                    case System.Net.HttpStatusCode.NotModified:
                        Debug.Log("Not Modified");
                        m_result = response.StatusCode + ": Not Modified";
                        break;
                    case System.Net.HttpStatusCode.BadRequest:
                        Debug.Log("Bad Request");
                        m_result = response.StatusCode + ": Bad Request";
                        break;
                    case System.Net.HttpStatusCode.Unauthorized:
                        Debug.Log("Unauthorized Error");
                        m_result = response.StatusCode + ": Unauthorized";
                        break;
                    case System.Net.HttpStatusCode.Forbidden:
                        Debug.Log("Forbidden");
                        m_result = response.StatusCode + ": Forbidden";
                        break;
                    case System.Net.HttpStatusCode.NotFound:
                        Debug.Log("Not Found");
                        m_result = response.StatusCode + ": Not Found";
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        Debug.Log("Internal Server Error");
                        m_result = response.StatusCode + ": Internal Server Error";
                        break;
                    case System.Net.HttpStatusCode.NotImplemented:
                        Debug.Log("Not Implemented");
                        m_result = response.StatusCode + ": Not Implemented";
                        break;
                    default:
                        Debug.Log("Unhandled status code: " + response.StatusCode);
                        break;
                }

                LLM m_LLM_Error_Response = new LLM
                {
                    // result = response.StatusCode + " \n " + response.ReasonPhrase
                    result = m_result
                };
                return m_LLM_Error_Response;
            }
        }
    }
}

public struct LLM
{
    public string result;
    public JArray history;
}
