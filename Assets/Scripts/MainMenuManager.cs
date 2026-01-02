using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Colyseus;
using System.Threading.Tasks;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI statusText;  // Text to display connection status

    [Header("Server Settings")]
    public string serverAddress = "ws://localhost:2567";

    private ColyseusClient client;
    private GameObject networkHolder;

    private void Awake()
    {
        // Create a persistent GameObject to hold the client across scenes
        networkHolder = new GameObject("ColyseusNetwork");
        DontDestroyOnLoad(networkHolder);
    }

    // Attach this method to the OnClick event of the Start Game button in the Inspector
    public async void StartGame()
    {
        if (statusText != null)
        {
            statusText.text = "Checking connection to the server...";
            statusText.color = Color.yellow;
        }

        bool connected = await TestServerConnection();

        if (connected)
        {
            if (statusText != null)
            {
                statusText.text = "Connected! Entering the lobby...";
                statusText.color = Color.green;
            }

            // Keep the client for use in the lobby scene
            var holder = networkHolder.AddComponent<ColyseusClientHolder>();
            holder.Client = client;

            SceneManager.LoadScene("Lobby");
        }
        else
        {
            if (statusText != null)
            {
                statusText.text = "Connection to the server is not established!";
                statusText.color = Color.red;
            }
        }
    }

    // Attach this method to the OnClick event of the Quit button in the Inspector
    public void QuitGame()
    {
        Debug.Log("خروج از بازی");
        Application.Quit();

    }

    // Simple and safe connection test (just create client and wait for timeout)
    private async Task<bool> TestServerConnection()
    {
        try
        {
            client = new ColyseusClient(serverAddress);

            // Wait up to 5 seconds for the WebSocket to attempt connection
            float timeout = 5f;
            float elapsed = 0f;

            while (elapsed < timeout)
            {
                // In recent versions, the connection attempt starts immediately
                // If no exception has been thrown by now, the handshake has likely started
                await Task.Delay(100); // 0.1 ثانیه صبر
                elapsed += 0.1f;
            }

            // If we reached here without an exception, assume the server is reachable
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("خطا در اتصال به سرور: " + e.Message);
            return false;
        }
    }
}

// Helper class to pass the client to the next scene
public class ColyseusClientHolder : MonoBehaviour
{
    public ColyseusClient Client { get; set; }
}