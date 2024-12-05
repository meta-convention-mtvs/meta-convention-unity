using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHallData : Singleton<MainHallData>
{
    public BoothCategory mainHallCategory;
    public string loadingSceneName;

    public void SetMainHallLoadingData(BoothCategory category, string loadingSceneName)
    {
        this.mainHallCategory = category;
        this.loadingSceneName = loadingSceneName;
    }



}
