using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Button))]
public class GameStartButtonInTitle : MonoBehaviour
{
    [SerializeField] private string sceneName;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            //TitleSceneManger.Instance.StartLoadScene(sceneName);
        });
    }
}
