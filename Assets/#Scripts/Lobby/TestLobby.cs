using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{

    Lobby _hostLobby, _joinedLobby;
    float _heartbeatTimer = 15f, _syncTimer = 2f;

    [SerializeField] TMP_InputField _lobbyCodeInputField;
    [SerializeField] TMP_Text _lobbyCodeText, _readyBtnText;

    [SerializeField] GameObject _joinedUI, _hostJoinUI, _playBtn;

    [SerializeField] TMP_Dropdown _playerModeDropdown;

    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += OnSignedIn;

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    private void Update()
    {
        HandleLobbyHeartbeat();

        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    ListLobbies();
        //}

        //if(Input.GetKeyDown(KeyCode.C))
        //{
        //    CreateLobby();
        //}

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            foreach (var player in _joinedLobby.Players)
            {
                if (player.Data == null)
                    Debug.Log("Player: " + player.Id + " null");
                else
                    Debug.Log("Player: " + player.Id + " " + player.Data["Mode"].Value);
            }
        }

        SyncLobby();
    }

    private async void SyncLobby()
    {
        if(_joinedLobby == null) return;

        _syncTimer -= Time.deltaTime;
        if (_syncTimer <= 0)
        {
            _syncTimer = 1.5f;
            var lobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);
            _joinedLobby = lobby;
        }

        LobbyUI.Instance.UpdateLobbyUI(_joinedLobby);
    }

    async void HandleLobbyHeartbeat()
    {
        if (_hostLobby != null)
        {
            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer <= 0)
            {
                _heartbeatTimer = 15f;
                await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
            }
        }
    }

    private void OnSignedIn()
    {
        Debug.Log("OnSignedIn: " + AuthenticationService.Instance.PlayerId);
    }

    public async void CreateLobby()
    {
        try {

            string lobbyName = "TestLobby";
            int maxPlayers = 2;

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>()
                    {
                        {
                            "Mode", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "0")
                        },
                        {
                            "Ready", new PlayerDataObject( PlayerDataObject.VisibilityOptions.Public, "0")
                        }
                    }
                },
            };

            var newLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            _hostLobby = newLobby;
            _joinedLobby = newLobby;

            _joinedUI.SetActive(true);
            _hostJoinUI.SetActive(false);

            _lobbyCodeText.text = "LOBBY CODE: " + _joinedLobby.LobbyCode;

            Debug.Log("CreateLobby: " + newLobby.Id + " " + newLobby.Name + " " + newLobby.LobbyCode);
        }catch(LobbyServiceException e)
        {
            Debug.Log("CreateLobby: " + e.Message);
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };


            var queryResponse = await LobbyService.Instance.QueryLobbiesAsync(options);

            Debug.Log("CountLobbies: " + queryResponse.Results.Count);

            var lobbies = queryResponse.Results;

            foreach (var lobby in lobbies)
            {
                Debug.Log("ListLobbies: " + lobby.MaxPlayers + " " + lobby.Name);
            }

        }catch(LobbyServiceException e)
        {
            Debug.Log("ListLobbies: " + e.Message);
        }
    }

    public async void JoinLobby()
    {
        try
        {
            var lobbyCode = _lobbyCodeInputField.text;
            

            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>()
                    {
                        {
                            "Mode", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "0")
                        },
                        {
                            "Ready", new PlayerDataObject( PlayerDataObject.VisibilityOptions.Public, "0")
                        }
                    }
                }
            };

            var joinResponse = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);

            _hostJoinUI.SetActive(false);
            _joinedUI.SetActive(true);

            _joinedLobby = joinResponse;

            _lobbyCodeText.text = "LOBBY CODE: " + lobbyCode;

            _playBtn.SetActive(false);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log("JoinLobby: " + e.Message);
        }
    }

    public async void UpdatePlayerMode()
    {
        try
        {
            var index = _playerModeDropdown.value.ToString();
            string playerId = AuthenticationService.Instance.PlayerId;
            var lobbyId = _joinedLobby.Id;

            Debug.Log("UpdatePlayerMode: " + index + " " + playerId + " " + lobbyId);

            var options = new UpdatePlayerOptions();

            options.Data = new Dictionary<string, PlayerDataObject>()
            {
                {
                    "Mode", new PlayerDataObject( PlayerDataObject.VisibilityOptions.Public, index)
                }
            };

            Debug.Log("UpdatePlayerMode: " + options.Data["Mode"].Value);

            var lobby = await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerId, options);


            _joinedLobby = lobby;

        }
        catch(LobbyServiceException e)
        {
            Debug.Log("UpdatePlayerMode: " + e);
        }
    }

    public void PlayBtn()
    {
        var players = _joinedLobby.Players;

        if (players.Count != 2)
        {
            Debug.Log("There must be 2 players in the lobby.");
            return;
        }

        foreach(var player in players)
        {
            if (player.Data == null || player.Data["Mode"].Value == "0")
            {
                Debug.Log("All players must choose what to play as.");
                return;
            }

            if (player.Data["Ready"].Value == "0")
            {
                Debug.Log("All players must be ready.");
                return;
            }
        }

        if (players[0].Data["Mode"].Value == players[1].Data["Mode"].Value)
        {
            Debug.Log("Players must choose different modes.");
            return;
        }



        Debug.Log("Play!");
    }

    public void ReadyBtn()
    {
        if (_joinedLobby == null) return;

        if (_readyBtnText.text == "READY")
        {
            _readyBtnText.text = "UNREADY";
            UpdatePlayerReady("1");
        }
        else
        {
            _readyBtnText.text = "READY";
            UpdatePlayerReady("0");
        }
    }

    private async void UpdatePlayerReady(string v)
    {
        try
        {
            string playerId = AuthenticationService.Instance.PlayerId;
            var lobbyId = _joinedLobby.Id;

            var options = new UpdatePlayerOptions();

            options.Data = new Dictionary<string, PlayerDataObject>()
            {
                {
                    "Ready", new PlayerDataObject( PlayerDataObject.VisibilityOptions.Public, v)
                }
            };

            var lobby = await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerId, options);
            _joinedLobby = lobby;

        }
        catch (LobbyServiceException e)
        {
            Debug.Log("UpdatePlayerMode: " + e);
        }
    }
}
