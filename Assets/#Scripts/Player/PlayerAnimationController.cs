using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimationController : NetworkBehaviour
{
    public static PlayerAnimationController Instance;

    Animator _animator;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            _animator = GetComponentInChildren<Animator>();
        }
    }

    public void PlayInteractionAnimation()
    {
        if (!IsOwner) return;

        _animator.SetTrigger("interact");
    }

    public void SetMovemetVariables(float vertical, float horizontal)
    {
        if (!IsOwner) return;

        _animator.SetFloat("horizontal", horizontal);
        _animator.SetFloat("vertical", vertical);

        var sum = Mathf.Abs(vertical) + Mathf.Abs(horizontal);

        _animator.SetBool("isRunning", sum > 0.0001 ? true : false);
    }
}
