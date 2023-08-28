using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using TMPro;

public class OnPlayerSpawn : NetworkBehaviour
{
    bool _isAllSet = false;
    [SerializeField] private Transform _playerCameraRoot;
    [SerializeField] TMP_Text _nickText;
    string _name;

    private void Start()
    {
        _isAllSet = false;
    }


    private void Update()
    {
        if (!IsOwner) return;

        if (_isAllSet) return;
        _isAllSet = true;
        _name = UnityEngine.Random.Range(0, 100).ToString();
        SetNickNameServerRpc(_name);
        SetMovementVariables();
    }

    private void SetMovementVariables()
    {
        var playerFollowCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        var uiCanvasControllerInput = GameObject.Find("MoevementUI").GetComponent<StarterAssets.UICanvasControllerInput>();

        playerFollowCamera.Follow = _playerCameraRoot;
        uiCanvasControllerInput.starterAssetsInputs = this.GetComponentInChildren<StarterAssets.StarterAssetsInputs>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetNickNameServerRpc(string nick)
    {
        SetNickNameClientRpc(nick);
    }

    [ClientRpc]
    public void SetNickNameClientRpc(string nick)
    {
        _nickText.text = nick;
    }
}
