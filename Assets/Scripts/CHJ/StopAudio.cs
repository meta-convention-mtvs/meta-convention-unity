using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAudio : MonoBehaviour
{
    public void Stop()
    {
        SoundMgr.Instance.StopAudio();
    }   
}
