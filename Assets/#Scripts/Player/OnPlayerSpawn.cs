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
        if (!IsOwner) return;
        Debug.Log("OnPlayerSpawn Start");
        _name = UnityEngine.Random.Range(0, 100).ToString();
    }


    private void Update()
    {
        if(_nickText.text != _name) _nickText.text = _name;

        if (!IsOwner) return;

        SetMovementVariables();
    }

    private void SetMovementVariables()
    {
        var playerFollowCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
        var uiCanvasControllerInput = GameObject.Find("MoevementUI").GetComponent<StarterAssets.UICanvasControllerInput>();

        playerFollowCamera.Follow = _playerCameraRoot;
        uiCanvasControllerInput.starterAssetsInputs = this.GetComponentInChildren<StarterAssets.StarterAssetsInputs>();
    }
}
