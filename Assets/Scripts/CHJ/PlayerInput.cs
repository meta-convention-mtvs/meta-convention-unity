using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInput : MonoBehaviour
{
    [Serializable]
    public class UserInput
{
    public float horizontal;
    public float vertical;
    public float mouseX;
    public float mouseY;
}

    public string currentControlScheme;
    [SerializeField]
    UserInput userInput;
    public UserInput _userInput
    {
        get
        {
            return userInput;
        }
    }

    private void Start()
    {
        userInput = new UserInput();
    }
    void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        userInput.horizontal = Input.GetAxis("Horizontal");
        userInput.vertical = Input.GetAxis("Vertical");
        userInput.mouseX = Input.GetAxis("Mouse X");
        userInput.mouseY = Input.GetAxis("Mouse Y");
    }

    
}
