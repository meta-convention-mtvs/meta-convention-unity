using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove_HJK : MonoBehaviour
{
    CharacterController cc;

    public float moveSpeed = 3.0f;
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }
    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        // input값을 받는다.
        Vector3 dirH = transform.right * h;
        Vector3 dirV = transform.forward * v;
        Vector3 dir = dirH + dirV;

        // 이동한다
        cc.Move(dir.normalized * moveSpeed * Time.deltaTime);
    }

}
