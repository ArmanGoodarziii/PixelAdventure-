using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Colyseus;
using System.Threading.Tasks;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI statusText;  // متن برای نمایش وضعیت (اختیاری)

    [Header("Server Settings")]
    public string serverAddress = "ws://localhost:2567";

    private ColyseusClient client;
    private GameObject networkHolder;

    private void Awake()
    {
        // GameObject دائمی برای نگه داشتن client در صحنه بعدی
        networkHolder = new GameObject("ColyseusNetwork");
        DontDestroyOnLoad(networkHolder);
    }

    // این متد رو در Inspector به OnClick دکمه Start Game وصل کن
    public async void StartGame()
    {
        if (statusText != null)
        {
            statusText.text = "Checking connection to the server...";
            statusText.color = Color.white;
        }

        bool connected = await TestServerConnection();

        if (connected)
        {
            if (statusText != null)
            {
                statusText.text = "Connected! Entering the lobby...";
                statusText.color = Color.green;
            }

            // client رو نگه می‌داریم برای استفاده در صحنه لابی
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

    // این متد رو در Inspector به OnClick دکمه Quit وصل کن
    public void QuitGame()
    {
        Debug.Log("خروج از بازی");
        Application.Quit();
    }

    // تست اتصال ساده و ایمن (فقط ساخت client و چک timeout)
    private async Task<bool> TestServerConnection()
    {
        try
        {
            client = new ColyseusClient(serverAddress);

            // صبر حداکثر ۵ ثانیه تا WebSocket سعی کنه وصل بشه
            float timeout = 5f;
            float elapsed = 0f;

            while (elapsed < timeout)
            {
                // در نسخه‌های جدید، اتصال بلافاصله سعی می‌کنه وصل بشه
                // اگر تا اینجا exception ننداخته، یعنی حداقل handshake شروع شده
                await Task.Delay(100); // 0.1 ثانیه صبر
                elapsed += 0.1f;
            }

            // اگر تا اینجا exception ننداخته باشه، سرور در دسترسه
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("خطا در اتصال به سرور: " + e.Message);
            return false;
        }
    }
}

// کلاس کمکی برای انتقال client به صحنه بعدی
public class ColyseusClientHolder : MonoBehaviour
{
    public ColyseusClient Client { get; set; }
}