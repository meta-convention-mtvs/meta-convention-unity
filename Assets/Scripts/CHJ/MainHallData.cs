using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHallData : Singleton<MainHallData>
{
    public BoothCategory mainHallCategory;
    public string loadingSceneName;
    public string targetCompanyUuid;

    public void SetMainHallLoadingData(BoothCategory category, string loadingSceneName)
    {
        this.mainHallCategory = category;
        this.loadingSceneName = loadingSceneName;
    }

    public void SetTargetCompanyUuid(string uuid)
    {
        this.targetCompanyUuid = uuid;
    }

}
