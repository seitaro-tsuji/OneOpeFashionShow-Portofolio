using UnityEngine;
using UnityEngine.UI;

//デバッグに使用するボタン
[RequireComponent (typeof(Button))]
public class DebugButton : MonoBehaviour
{
    private Button _button;
    [SerializeField] private int debugNumber = 0;
    [SerializeField, TextArea(6,10)] private string _memo;

    private void Awake()
    {
        _button = GetComponent<Button>();

        switch (debugNumber)
        {
            case 0:
                _button.onClick.AddListener(PlaySe);
                break;
            case 1:
                _button.onClick.AddListener(() => GameSceneManager.Instance.ChangeScene("ResultScene"));
                break;
            case 2:
                _button.onClick.AddListener(Debug2);
                break;
            case 3:
                _button.onClick.AddListener(() => GameSceneManager.Instance.ChangeScene("InGameTest"));
                break;
            default:
                break;
                
        }
    }

    private void PlaySe()
    {
        AudioManager.Instance.PlaySEOneShot(ClipName.GameStartSE);
    }

    private void Debug2()
    {
        GameSceneManager.Instance.ChangeScene("ResultScene");
        GameSceneManager.Instance.SetResultScore(100, 200, 300);
    }
}
