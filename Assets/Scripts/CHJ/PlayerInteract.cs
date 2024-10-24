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


    private void Update()
    {
        if (photonView != null && !photonView.IsMine)
            return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionDistance, interactionMask);

        GameObject closestObject = FindClosestGameObject(colliders, interactionDistance);


        if(previousClosestObject != null && closestObject != previousClosestObject)
        {
            IKeyInteractableObject go = previousClosestObject.GetComponent<IKeyInteractableObject>();
            go.HideText();
        }

        if (closestObject != null)
        {
            IKeyInteractableObject go = closestObject.GetComponent<IKeyInteractableObject>();
            go.ShowText();
            if (Input.GetKeyDown(KeyCode.F))
            {
                go.Interact();
            }
        }
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


}
