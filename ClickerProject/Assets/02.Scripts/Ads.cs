using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Monetization;

public class Ads : MonoBehaviour
{
    public string placementld = "rewardedVideo";

    // 전처리 코드
    // 해당 내용은 해당 OS일 때 실행할 수 있는 문법
#if UNITY_IOS
    private string gameId = "3222147";
#elif UNITY_ANDROID
    private string gameId = "3222146";
#elif UNITY_EDITOR
    private string gameId = "7654321";
#else
    private string gameId = "7654321";
#endif

    // Start is called before the first frame update
    void Start()
    {
        if (Monetization.isSupported)
        {
            Monetization.Initialize(gameId, true); // <- 테스트 모드. 실제로 출시할 때 false로 하자
        }
    }

    // 광고를 실행하는 함수
    public void ShowAd()
    {
        ShowAdCallbacks options = new ShowAdCallbacks();
        options.finishCallback = HandleShowResult;

        ShowAdPlacementContent ad = Monetization.GetPlacementContent(placementld)
            as ShowAdPlacementContent; // placementId 는 광고에 대한 설정이 담겨있다. 스킵을 허용하는지 안하는지 

        ad.Show(options);
    }

    // 결과를 처리하는 함수
    void HandleShowResult(ShowResult result)
    {
        if (result == ShowResult.Finished) // 광고재생을 스킵없이 모두 마쳤을 때
        {
            GameManager.money += 1000;
        }
        else if (result == ShowResult.Skipped) // 스킵했을 때
        {

        }
        else // 실패
        {

        }
    }
}
