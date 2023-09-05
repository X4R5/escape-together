using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{

    [SerializeField] float _movementSpeed = 5f, _fastMovementSpeed = 7f, _rotationSpeed = 5f;
    float _currentMovementSpeed;
    Transform _playerObj;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            foreach (var _transform in GetComponentsInChildren<Transform>())
            {
                if (_transform == this.transform) continue;
                _playerObj = _transform;
                break;
            }
        }
        Debug.Log("PlayerMovement OnNetworkSpawn playerObj: " + _playerObj.name);
    }

    void Update()
    {
        if (!IsOwner) return;

        Move();
    }

    void Move()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _currentMovementSpeed = _fastMovementSpeed;
        }
        else
        {
            _currentMovementSpeed = _movementSpeed;
        }

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var movement = new Vector3(horizontal, 0, vertical);

        transform.Translate(movement * Time.deltaTime * _currentMovementSpeed);

        if (movement != Vector3.zero)
        {
            _playerObj.rotation = Quaternion.Slerp(_playerObj.rotation, Quaternion.LookRotation(movement), Time.deltaTime * _rotationSpeed);
        }
    }
}
