using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class TalkingMovement : MonoBehaviour
{
    public AudioSource talkingSource;

    public Transform inScreen;
    public Transform outScreen;
    public float moveSpeed = 5f;

    void Start()
    {
        
    }

    void Update()
    {
        if (talkingSource.isPlaying)
        {
            transform.position = Vector3.Lerp(transform.position, inScreen.position, moveSpeed * Time.deltaTime);
        } 
        else if(!talkingSource.isPlaying)
        {
            transform.position = Vector3.Lerp(transform.position, outScreen.position, moveSpeed * Time.deltaTime);
        }
    }



    // 대화 하고 있을 때 말 풍선에 글을 띄우고 
    // 화면 안으로 들어온다. 
    //
    // Lerf로..
    // inScreen  <--> outScreen 

    // 판넬 만들어서 Raw Image 랑 붙여서 같이 움직이게 하기
    // gameobject 째로.. 묶어서

}
