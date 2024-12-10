using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI2.Recommend
{
    public class RecommendPanelRow : MonoBehaviour
    {
        public int Capacity => 3;

        public int ItemAmount { get; private set; }

        public void View(RecommendItem item, string loadingSceneName)
        {
            GetComponentsInChildren<RecommendPanelItem>()[ItemAmount].SetItem(item);
            GetComponentsInChildren<RecommendPanelItem>()[ItemAmount].LoadingScene = loadingSceneName;
            ItemAmount++;
        }
    }
}