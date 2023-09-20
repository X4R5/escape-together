using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractObject : NetworkBehaviour
{
    bool _canInteract = false;
    [SerializeField] GameObject _setActiveObject;

    private void Update()
    {
        if(!IsOwner) return;
        if(!_canInteract) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            _setActiveObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canInteract = true;
        }
    }
}
