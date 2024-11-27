using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    public int index;
    private void Start()
    {
        PlayAudio(index);
    }
    void PlayAudio(int index)
    {
        SoundMgr.Instance.idx = index;
    }
}
