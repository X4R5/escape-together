using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class ServerUI : MonoBehaviour
{
    [SerializeField] Button _serverBtn, _hostBtn, _clientBtn;

    private void Awake()
    {
        _serverBtn.onClick.RemoveAllListeners();
        _hostBtn.onClick.RemoveAllListeners();
        _clientBtn.onClick.RemoveAllListeners();

        _serverBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        _hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        _clientBtn.onClick.AddListener(() => { 
            NetworkManager.Singleton.StartClient();
        });
    }
}
