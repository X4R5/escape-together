using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;

    private async void Awake()
    {
        Instance = this;

        await UnityServices.InitializeAsync();
    }

    public async Task<string> CreateRelay()
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(1);

            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            var relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log("CreateRelay: " + e.Message);
            return null;
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            var relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log("JoinRelay: " + e.Message);
        }
    }
}
