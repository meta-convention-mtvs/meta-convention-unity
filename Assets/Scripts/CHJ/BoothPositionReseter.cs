using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHJ;
using System.Threading.Tasks;
using System;

public class BoothPositionReseter : Singleton<BoothPositionReseter>
{
    public int currentIndex;
    public string uuid;

    public bool isBoothPositionResetNeed = false;
    public Action OnSaveData;

    public void SetValue(int currentIndex, string uuid)
    {
        this.currentIndex = currentIndex;
        this.uuid = uuid;
        isBoothPositionResetNeed = true;
    }

    // TODO : When application end, this methods should be played;
    public async Task SaveDataWhileQuit()
    {
        await SaveChargedBoothPosition(currentIndex, uuid);

        OnSaveData?.Invoke();
    }

    private async Task<bool> SaveChargedBoothPosition(int index, string uuid)
    {
        // 서버에 저장
        ChargedBoothPosition position = await AsyncDatabase.GetDataFromDatabase<ChargedBoothPosition>(DatabasePath.GetPublicDataPath(nameof(ChargedBoothPosition)));
        position.BoothPositionList[index] = new ChargedBoothData(false, "");
        DatabaseManager.Instance.SavePublicData<ChargedBoothPosition>(position);
        return true;
    }
}
