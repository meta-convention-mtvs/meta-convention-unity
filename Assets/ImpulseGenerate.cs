using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseGenerate : MonoBehaviour
{
    public CinemachineImpulseSource impulse;

    private void Start()
    {
        impulse = GetComponent<CinemachineImpulseSource>();
        impulse.GenerateImpulse();
    }

}
