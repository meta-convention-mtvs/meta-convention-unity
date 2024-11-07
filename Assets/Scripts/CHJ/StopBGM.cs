using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopBGM : MonoBehaviour
{
    public void Stop()
    {
        SoundMgr.Instance.StopAudio();
    }
}
