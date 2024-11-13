using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviourPun
{
    public float interactionDistance = 1.0f;
    public LayerMask interactionMask;

    GameObject previousClosestObject;
    GameObject interactingObject;

    private void Update()
    {
        if (photonView != null && !photonView.IsMine)
            return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionDistance, interactionMask);

        foreach(Collider collider in colliders)
            Debug.Log(collider.name);
        // interaction종료를 확인하는 코드
        if (interactingObject != null && !isCollidersHave(colliders, interactingObject))
        {
            interactingObject.GetComponent<IKeyInteractableObject>().InteractEnd();
            interactingObject = null;
        }


        GameObject closestObject = FindClosestGameObject(colliders, interactionDistance);
        Debug.Log(closestObject?.name);
        // 만약 이전에 가장 가까운 오브젝트가 존재하고, 그 오브젝트가 현재 오브젝트랑 다를 때
        if(closestObject != null && closestObject != previousClosestObject)
        {
            // 이전 오브젝트의 Text를 숨기는 코드
            //IKeyInteractableObject go = previousClosestObject?.GetComponent<IKeyInteractableObject>();
            //go?.HideText();

            // 현재 오브젝트의 Text를 보여주는 코드
            closestObject.GetComponent<IKeyInteractableObject>()?.ShowText();
        }

        // 만약 가장 가까운 오브젝트가 존재하고, F키가 눌렸으면
        if (closestObject != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                IKeyInteractableObject go = closestObject.GetComponent<IKeyInteractableObject>();
                go.Interact();
                interactingObject = closestObject;
            }
        }

        // 이전 오브젝트를 갱신한다.
        previousClosestObject = closestObject;
    }

    GameObject FindClosestGameObject(Collider[] colliders, float range)
    {
        GameObject closestObject = null;
        float minDistance = range;

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                float distance = Vector3.Distance(gameObject.transform.position, collider.gameObject.transform.position);
                Debug.Log(distance + " " +minDistance);
                if (distance < minDistance)
                {
                    closestObject = collider.gameObject;
                    minDistance = distance;
                }
            }
            else
            { 
                continue;
            }
        }

        return closestObject;
    }

    bool isCollidersHave(Collider[] colliders, GameObject value)
    {
        foreach(Collider collider in colliders)
        {
            if(collider.gameObject == value)
            {
                return true;
            }
        }
        return false;
    }
}
