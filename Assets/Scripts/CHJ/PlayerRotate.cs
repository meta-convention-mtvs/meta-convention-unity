using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerRotate : MonoBehaviour
{
    PlayerInput playerInput;

    public float rotSpeed = 200.0f;
    public bool useRotX, useRotY;
    float rotX, rotY;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        // 1. 회전 값을 변경 (누적)
        if (useRotY)
        {
            rotX += playerInput._userInput.mouseY * rotSpeed * Time.deltaTime;
            // 2. rotX 의 값을 제한(최소값, 최대값)
            rotX = Mathf.Clamp(rotX, -80, 80);
        }
        if (useRotX)
        {
            rotY += playerInput._userInput.mouseX * rotSpeed * Time.deltaTime;
        }

        // 3. 구해진 회전 값을 나의 회전 값으로 셋팅
        transform.localEulerAngles = new Vector3(-rotX, rotY, 0);
    }
}
