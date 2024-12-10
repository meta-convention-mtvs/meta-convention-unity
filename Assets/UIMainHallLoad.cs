using Ricimi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SceneTransition))]
public class UIMainHallLoad : MonoBehaviour
{
    public Button[] buttons;
    public string LoadingScene = "Start_Universe";

    void Start()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            int j = i;
            buttons[i].onClick.AddListener(() => { MainHallData.Instance.SetMainHallLoadingData((BoothCategory)j, LoadingScene); PerformTransition(LoadingScene); buttons[i].interactable = false; });
        }
    }

    void PerformTransition(string LoadingScene)
    {
        SceneTransition sceneTransition = GetComponent<SceneTransition>();
        sceneTransition.scene = LoadingScene;
        sceneTransition.PerformTransition();
    }
}
