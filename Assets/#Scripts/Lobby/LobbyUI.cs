using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance;

    [SerializeField] Toggle _readyToggleP1, _readyToggleP2;

    private void Awake()
    {
        Instance = this;
    }

    internal void UpdateLobbyUI(Lobby joinedLobby)
    {
        if (joinedLobby == null) return;

        var players = joinedLobby.Players;

        if (players.Count > 0 && players[0].Data["Ready"].Value == "1")
        {
            _readyToggleP1.isOn = true;
        }else
        {
            _readyToggleP1.isOn = false;
        }

        if (players.Count > 1 && players[1].Data["Ready"].Value == "1")
        {
            _readyToggleP2.isOn = true;
        }
        else
        {
            _readyToggleP2.isOn = false;
        }
    }

    void ToggleReadyBox(Lobby lobby, string id)
    {
        var players = lobby.Players;

        if (players[0].Id == id)
        {
            _readyToggleP1.isOn = !_readyToggleP1.isOn;
        }
        else
        {
            _readyToggleP2.isOn = !_readyToggleP2.isOn;
        }
    }
}
