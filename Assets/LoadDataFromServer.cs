using Ricimi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadDataFromServer : MonoBehaviour
{

    public SceneTransition cardTransition;
    public SceneTransition universeTransition;

    // 로그인 정보를 바탕으로 데이터베이스에서 데이터를 불러온다.
    Card userCardData;
    CharacterCustomizeData userCharacterCustomizeData;

    bool cardDataLoadedComplete;
    bool customizeDataLoadedComplete;

    bool isSceneLoading = false;

    private void Start()
    {
        userCardData = new Card();
        userCharacterCustomizeData = new CharacterCustomizeData();
    }

    private void Update()
    {
        if(cardDataLoadedComplete && customizeDataLoadedComplete && !isSceneLoading)
        {
            if(userCardData == null || userCharacterCustomizeData == null)
            {
                cardTransition.PerformTransition();
            }
            else
            {
                universeTransition.PerformTransition();
            }
            isSceneLoading = true;
        }
    }
    // 데이터가 있으면 넘어간다. 
    // 데이터가 없으면 만든다.
    public void LoadUserDataFromServer()
    {
        DatabaseManager.Instance.GetData<Card>((data) => { userCardData = data; cardDataLoadedComplete = true; });
        DatabaseManager.Instance.GetData<CharacterCustomizeData>((data) => { userCharacterCustomizeData = data; customizeDataLoadedComplete = true; });

    }

}
