using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class InteractObject : NetworkBehaviour
{
    [SerializeField] GameObject _setActiveObject;

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (!IsOwner) return;

        _setActiveObject.SetActive(true);
    }
}
