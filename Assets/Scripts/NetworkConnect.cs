using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class NetworkConnect : MonoBehaviour
{
    public string joinCode;
    public int maxConnection = 20;
    public UnityTransport transport;

    private Lobby currentLobby;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void Create() 
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
        string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        Debug.LogError(newJoinCode);

        transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, 
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

        //CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
        //lobbyOptions.IsPrivate = false;
        //lobbyOptions.Data = new Dictionary<string, DataObject>();
        //DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, newJoinCode);
        //lobbyOptions.Data.Add("JOIN_CODE", dataObject);

        //currentLobby = await LobbyService.Instance.CreateLobbyAsync("Lobby Name", maxConnection, lobbyOptions);

        NetworkManager.Singleton.StartHost();
    }

    public async void Join() 
    {
        //currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
        //string relayJoinCode = currentLobby.Data["JOIN_CODE"].Value;

        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, 
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }
}
