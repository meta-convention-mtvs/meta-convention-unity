using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashedDataFromDatabase : Singleton<CashedDataFromDatabase>
{
    public Language playerLanguage;
    public CharacterTopBottomCustomizeData playerCustomizeData;
    public Card playerInfo;
    public RecommendedCompanyListData playerRecommendedCompanyListData;

    [SerializeField]
    private bool isLanguageLoaded;
    [SerializeField]
    private bool isCharacterLoaded;
    [SerializeField]
    private bool isInfoLoaded;
    [SerializeField]
    private bool isCompanyListLoaded;

    private void Start()
    {
        CashDataFromDatabase();
    }
    void CashDataFromDatabase()
    {
        DatabaseManager.Instance.GetData<Language>(OnPlayerLanguageLoaded);
        DatabaseManager.Instance.GetData<CharacterTopBottomCustomizeData>(OnPlayerCharacterLoaded);
        DatabaseManager.Instance.GetData<Card>(OnPlayerInfoLoaded);
        DatabaseManager.Instance.GetData<RecommendedCompanyListData>(OnPlayerRecommendedCompanyListDataLoaded);
    }

    void OnPlayerLanguageLoaded(Language language)
    {
        playerLanguage = language;
        isLanguageLoaded = true;
    }

    void OnPlayerCharacterLoaded(CharacterTopBottomCustomizeData customizeData)
    {
        playerCustomizeData = customizeData;
        isCharacterLoaded = true;
    }

    void OnPlayerInfoLoaded(Card playerInfo)
    {
        this.playerInfo = playerInfo;
        isInfoLoaded = true;
    }

    void OnPlayerRecommendedCompanyListDataLoaded(RecommendedCompanyListData recommendedCompanyListData)
    {
        playerRecommendedCompanyListData = recommendedCompanyListData;
        isCompanyListLoaded = true;
    }
}
