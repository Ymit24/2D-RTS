using UnityEngine;

using Game.Net;
using Game.Net.NetMessages;

public class NetworkController : MonoBehaviour {
    private static INetClient client;

    public static INetClient Client
    {
        get
        {
            return client;
        }
    }

    public static void Connect()
    {
        client = new SocketClient();
        client.ConnectToServer();
    }

    public static void RequestToken()
    {
        client.RequestToken();
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (client != null)
        {
            client.Process();
        }
    }

    private void OnDestroy()
    {
        client.Disconnect();
    }
}
