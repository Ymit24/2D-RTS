using UnityEngine;

using Game.Lobby;
using Game.Net;
using Game.Net.NetMessages;

public class LobbyController : MonoBehaviour
{
    private LobbyClient lobbyClient;
    [SerializeField] private TMPro.TextMeshProUGUI inputField;

    public void JoinLobby()
    {
        Debug.Log("Name: " + inputField.text);

        NetworkController.Connect();

        lobbyClient = new LobbyClient(NetworkController.Client, inputField.text);
        lobbyClient.Init();

        lobbyClient.OnNewClientJoined += OnClientJoined;
        lobbyClient.OnDisconnected += OnDisconnected;
        lobbyClient.OnClientReadyChanged += OnReadyChanged;

        NetworkController.Client.AddCallback(NetOP.AssignToken, (NetMsg msg) =>
        {
            lobbyClient.Join();
        });

        NetworkController.RequestToken();
    }

    private void OnClientJoined(ClientData client)
    {
        Debug.Log("Client joined.");
    }

    private void OnDisconnected(ClientData client)
    {
        Debug.Log("Client disconnected.");
    }

    private void OnReadyChanged(ClientData client)
    {
        Debug.Log("Client changed ready status.");
    }
}