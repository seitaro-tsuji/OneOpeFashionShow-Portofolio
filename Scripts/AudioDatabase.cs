using System;
using System.Collections.Generic;
using UnityEngine;

public enum ClipName
{
    TitleBGM,
    InGameBGM,
    ResultBGM,
    FassionChangeSE,
    GameFinishSE,
    GameStartSE,
    GoSE,
    HoldClothSE,
    OrderFailureSE,
    OrderSuccessSE,
    PurchaseSE,
    SelectSE
}

[Serializable]
public class AudioData
{
    public ClipName nameEnum;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "Scriptable Objects/AudioDatabase")]
public class AudioDatabase : ScriptableObject
{
    [SerializeField] private List<AudioData> audioDatas = new List<AudioData>();

    public AudioClip GetClip(ClipName nameEnum)
    {
        foreach (AudioData audioData in audioDatas)
        {
            if(nameEnum == audioData.nameEnum)
            {
                return audioData.clip;
            }
        }

        Debug.LogWarning($"クリップが見つかりません。{nameEnum}");
        return null;
    }
}
