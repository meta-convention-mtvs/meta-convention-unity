using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    CharacterController cc;
    PlayerInput playerInput;

    public float moveSpeed = 3.0f;
    void Start()
    {
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Move();
    }
    void Move()
    {
        // input���� �޴´�.
        Vector3 dirH = transform.right * playerInput._userInput.horizontal;
        Vector3 dirV = transform.forward * playerInput._userInput.vertical;
        Vector3 dir = dirH + dirV;

        // �̵��Ѵ�
        cc.Move(dir.normalized * moveSpeed * Time.deltaTime);
    }

}
