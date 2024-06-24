using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;
using System.Threading.Tasks;
using System;
using TMPro;

public class WebBrowser : MonoBehaviour
{
    [SerializeField] private CanvasWebViewPrefab canvasWebViewPrefab;
    [SerializeField] private Button backButton;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button reloadButton;
    [SerializeField] private Button searchButton;  // New search button
    [SerializeField] private TMP_InputField urlInputField;

    private void Start()
    {
        // Initialize the webview
        canvasWebViewPrefab.Initialized += OnWebViewInitialized;

        // Set up button listeners
        backButton.onClick.AddListener(OnBackButtonClicked);
        forwardButton.onClick.AddListener(OnForwardButtonClicked);
        reloadButton.onClick.AddListener(OnReloadButtonClicked);
        searchButton.onClick.AddListener(OnSearchButtonClicked);  // Set up search button listener
        urlInputField.onEndEdit.AddListener(OnUrlInputFieldEndEdit);

        // Update UI elements based on WebView state
        canvasWebViewPrefab.WebView.LoadProgressChanged += OnLoadProgressChanged;
    }

    private void OnWebViewInitialized(object sender, EventArgs e)
    {
        canvasWebViewPrefab.WebView.LoadUrl("https://www.google.com");
    }

    private async void OnBackButtonClicked()
    {
        if (await canvasWebViewPrefab.WebView.CanGoBack())
        {
            canvasWebViewPrefab.WebView.GoBack();
        }
    }

    private async void OnForwardButtonClicked()
    {
        if (await canvasWebViewPrefab.WebView.CanGoForward())
        {
            canvasWebViewPrefab.WebView.GoForward();
        }
    }

    private void OnReloadButtonClicked()
    {
        canvasWebViewPrefab.WebView.Reload();
    }

    private void OnSearchButtonClicked()
    {
        LoadUrlFromInputField();
    }

    private void OnUrlInputFieldEndEdit(string url)
    {
        LoadUrlFromInputField();
    }

    private void LoadUrlFromInputField()
    {
        string url = urlInputField.text;
        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
        {
            url = "http://" + url;
        }
        canvasWebViewPrefab.WebView.LoadUrl(url);
    }

    private void OnLoadProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        // Update the URL input field when a new page is loaded
        if (e.Progress == 1.0f) // Page load complete
        {
            urlInputField.text = canvasWebViewPrefab.WebView.Url;
        }
    }
}
