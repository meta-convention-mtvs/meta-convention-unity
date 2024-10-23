using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
public class PlayerMove_HJK : MonoBehaviour
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
        // input값을 받는다.
        Vector3 dirH = transform.right * playerInput._userInput.horizontal;
        Vector3 dirV = transform.forward * playerInput._userInput.vertical;
        Vector3 dir = dirH + dirV;

        // 이동한다
        cc.Move(dir.normalized * moveSpeed * Time.deltaTime);
    }

}
