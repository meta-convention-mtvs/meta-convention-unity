using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHJ;
using System.Threading.Tasks;
using System;

public class CashedDataFromDatabase : Singleton<CashedDataFromDatabase>
{
    public Language playerLanguage;
    public CharacterTopBottomCustomizeData playerCustomizeData;
    public Card playerInfo;
    public RecommendedCompanyListData playerRecommendedCompanyListData;

    public bool allDataCashed { get; private set; }

    public Action OnCashedData;
    
    private void Start()
    {
        CashDataFromDatabase();       
    }
    async Task CashDataFromDatabase()
    {
        var uid = FireAuthManager.Instance.GetCurrentUser().UserId;

        playerLanguage = await AsyncDatabase.GetDataFromDatabase<Language>(DatabasePath.GetUserDataPath(uid, nameof(Language)));
        playerCustomizeData = await AsyncDatabase.GetDataFromDatabase<CharacterTopBottomCustomizeData>(DatabasePath.GetUserDataPath(uid, nameof(CharacterTopBottomCustomizeData)));
        playerInfo = await AsyncDatabase.GetDataFromDatabase<Card>(DatabasePath.GetUserDataPath(uid, nameof(Card)));
        playerRecommendedCompanyListData = await AsyncDatabase.GetDataFromDatabase<RecommendedCompanyListData>(DatabasePath.GetUserDataPath(uid, nameof(RecommendedCompanyListData)));

        allDataCashed = true;
        OnCashedData?.Invoke();

    }
}
