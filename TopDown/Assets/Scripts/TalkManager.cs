using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;
    Dictionary<int, Sprite> portraitData;

    public Sprite[] portraitArr;

    private void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, Sprite>();
        GenerateData();
    }

    void GenerateData()
    {
        talkData.Add(1000, new string[] { "안녕하세요:0", "NPC입니다:1"});
        talkData.Add(2000, new string[] { "넌누구냐:0", "얼른꺼져:1" });
        talkData.Add(100, new string[] { "평범한 나무상자다" });

        // Quest Talk
        talkData.Add(10 + 1000, new string[] { "어서와:0",
            "여기 마을은 처음이지:1",
            "한 번 둘러보도록 해:0" });

        talkData.Add(11 + 2000, new string[] { "여어:1",
        "이 호수의 전설을 들으러 온 거야?:0",
        "그럼 일 좀 하나 해주면 좋을텐데:1",
        "내 집 근처:3"});

        talkData.Add(20 + 1000, new string[] { "루도의 동전?:1",
        "돈을 흘리고 다니면 못 쓰지?:3"});
        talkData.Add(20 + 2000, new string[] { "찾으면 꼭 좀 가져다 줘:1" });
        talkData.Add(10 + 2000, new string[] { });

        portraitData.Add(1000 + 0, portraitArr[0]);
        portraitData.Add(1000 + 1, portraitArr[1]);
        portraitData.Add(1000 + 2, portraitArr[2]);
        portraitData.Add(1000 + 3, portraitArr[3]);
        portraitData.Add(2000 + 0, portraitArr[4]);
        portraitData.Add(2000 + 1, portraitArr[5]);
        portraitData.Add(2000 + 2, portraitArr[6]);
        portraitData.Add(2000 + 3, portraitArr[7]);
    }

    public string GetTalk(int id, int talkIndex)
    {
        if (talkIndex == talkData[id].Length)
            return null;
        return talkData[id][talkIndex];
    }

    public Sprite GetPortrait(int id, int portraitindex)
    {
        return portraitData[id + portraitindex];
    }
}
