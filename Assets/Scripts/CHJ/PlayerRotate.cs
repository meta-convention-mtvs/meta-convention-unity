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
        // 1. ȸ�� ���� ���� (����)
        if (useRotY)
        {
            rotX += playerInput._userInput.mouseY * rotSpeed * Time.deltaTime;
            // 2. rotX �� ���� ����(�ּҰ�, �ִ밪)
            rotX = Mathf.Clamp(rotX, -80, 80);
        }
        if (useRotX)
        {
            rotY += playerInput._userInput.mouseX * rotSpeed * Time.deltaTime;
        }

        // 3. ������ ȸ�� ���� ���� ȸ�� ������ ����
        transform.localEulerAngles = new Vector3(-rotX, rotY, 0);
    }
}
