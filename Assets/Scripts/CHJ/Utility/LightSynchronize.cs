using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSynchronize : MonoBehaviour
{

    void Start()
    {
        DynamicGI.UpdateEnvironment();
    }

}
