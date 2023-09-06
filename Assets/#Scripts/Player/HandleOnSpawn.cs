using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class HandleOnSpawn : NetworkBehaviour
{
    Transform _librarianSpawnPoint, _adventurerSpawnPoint;
    GameObject _librarianCamera, _adventurerCamera;

    public override async void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            await SetAllVariables();

            var x = LobbyManager.Instance.playerModes[AuthenticationService.Instance.PlayerId];

            if (x == "1")
            {
                transform.position = _librarianSpawnPoint.position;
                _librarianCamera.SetActive(true);
                _adventurerCamera.SetActive(false);
            }
            else if (x == "2")
            {
                transform.position = _adventurerSpawnPoint.position;
                _librarianCamera.SetActive(false);
                _adventurerCamera.SetActive(true);
            }
        }
    }

    Task SetAllVariables()
    {
        _librarianSpawnPoint = GameObject.Find("LibrarianSpawnPoint").transform;
        _adventurerSpawnPoint = GameObject.Find("AdventurerSpawnPoint").transform;
        _librarianCamera = GameObject.Find("LibrarianCamera");
        _adventurerCamera = GameObject.Find("AdventurerCamera");

        return Task.CompletedTask;
    }
}
