using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecommendBoothDirection : MonoBehaviour
{
    
    public Vector3 boothDirection;
    public float boothDistance;

    Transform recommendedBoothPosition;


    private void Start()
    {
        recommendedBoothPosition = MainHallData.Instance.targetCompanyUuid;
    }

    private void Update()
    {
        boothDirection = recommendedBoothPosition.position - transform.position;
        boothDistance = boothDirection.magnitude;
        boothDirection.Normalize();
    }
}
