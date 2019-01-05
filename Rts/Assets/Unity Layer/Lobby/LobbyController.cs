using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Game.Lobby;
using Game.Net;
using Game.Net.NetMessages;

public class LobbyController : MonoBehaviour
{
    private LobbyClient lobbyClient;
    [SerializeField] private TMPro.TextMeshProUGUI inputField;
    [SerializeField] private GameObject LoginPanel;
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private GameObject ClientInfoPrefab;

    [SerializeField] private Transform ClientInfoList;

    private Dictionary<short, GameObject> tokenToClientInfo = new Dictionary<short, GameObject>();

    private void Start()
    {
        LoginPanel.SetActive(true);
        LobbyPanel.SetActive(false);
    }

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

            LoginPanel.SetActive(false);
            LobbyPanel.SetActive(true);
        });

        NetworkController.RequestToken();
    }

    /// <summary>
    /// This is called from the Unity UI
    /// </summary>
    public void ToggleReady()
    {
        lobbyClient.SetReady(!lobbyClient.Ready);
    }

    private void OnClientJoined(ClientData client)
    {
        Debug.Log("Client joined. Ready status : " + client.Ready);
        GameObject clientInfo = Instantiate(ClientInfoPrefab);
        clientInfo.transform.SetParent(ClientInfoList);
        clientInfo.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = client.Name;

        if (client.Token == NetworkController.Client.GetToken())
        {
            ClientInfoData data = clientInfo.GetComponent<ClientInfoData>();
            Button buttonCheck = data.Checkmark.GetComponent<Button>();
            if (buttonCheck != null)
            {
                buttonCheck.onClick.AddListener(ToggleReady);
            }
            Button buttonX = data.Xmark.GetComponent<Button>();
            if (buttonX != null)
            {
                buttonX.onClick.AddListener(ToggleReady);
            }
        }
        
        tokenToClientInfo.Add(client.Token, clientInfo);

        OnReadyChanged(client);
    }

    private void OnDisconnected(ClientData client)
    {
        Debug.Log("Client disconnected.");
        if (tokenToClientInfo.ContainsKey(client.Token))
        {
            Destroy(tokenToClientInfo[client.Token]);
        }
        else
        {
            Debug.LogWarning("LobbyController-OnDisconnected::Client disconnected but couldn't find it in the tokenToClient dictionary.");
        }
    }

    private void OnReadyChanged(ClientData client)
    {
        Debug.Log("Client changed ready status.");

        if (tokenToClientInfo.ContainsKey(client.Token))
        {
            GameObject clientGO = tokenToClientInfo[client.Token];
            ClientInfoData data = clientGO.GetComponent<ClientInfoData>();
            data.Checkmark.SetActive(client.Ready);
            data.Xmark.SetActive(!client.Ready);
        }
        else
        {
            Debug.LogWarning("LobbyController-OnReadyChanged::Client not found in dictionary.");
        }
    }
}