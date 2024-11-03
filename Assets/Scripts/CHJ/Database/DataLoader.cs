using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    // 데이터 베이스에서 데이터를 읽어온다.
    // 읽은 데이터를 don't destory on load로 설정

    private void Start()
    {
        // 로그인 되었을 때 함수를 설정한다.
        FireAuthManager.Instance.OnLogin += LoadUserDataFromServer;
    }

    public void LoadUserDataFromServer()
    {
        Card userCardData = new Card();
        CharacterCustomizeData userCharacterCustomizeData = new CharacterCustomizeData();
        DatabaseManager.Instance.GetData<Card>((data) => { userCardData = data; });
        DatabaseManager.Instance.GetData<CharacterCustomizeData>((data) => { userCharacterCustomizeData = data;});


        GameObject go = new GameObject("UserData");
        UserData userData = go.AddComponent<UserData>();
        
    }

}
