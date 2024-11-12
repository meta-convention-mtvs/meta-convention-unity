using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBooth : MonoBehaviour
{
    public Transform[] BoothPosition;
    public Action<GameObject> OnBoothCreate;

    GameObject booth;

    private void Start()
    {
        // 만약 내가 기업 유저이다
        // 부스를 만든다.
        // 부스 위치 정보를 읽어온다.
        // 부스 위치 정보에 맞는 위치에 부스를 생성한다.

    }
}
