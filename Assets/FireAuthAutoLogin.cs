using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAuthAutoLogin : MonoBehaviour
{
    void Start()
    {
        FireAuthManager.Instance.LogIn("test@test6.com", "12345678", () => Debug.Log("Sign success from AutoLogin"), (s)=> Debug.Log("Sign failed form AutoLogin") );
    }

}
