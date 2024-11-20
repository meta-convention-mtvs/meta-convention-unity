using Ricimi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionWhenLogin : MonoBehaviour
{

    public SceneTransition transition;
    private void Start()
    {
        FireAuthManager.Instance.OnLogin += transition.PerformTransition;
    }


}
