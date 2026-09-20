using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    [SerializeField] private float _fadeOutAndInDuration = 1f;
    private Button _button;

    private void Awake()
    {
        if(_sceneName == null)
        {
            Debug.LogError($"sceneName‚ªnull‚Å‚·:{gameObject}");
        }

        _button = GetComponent<Button>();
        _button.onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        GameSceneManager.Instance.ChangeScene(_sceneName);
        AudioManager.Instance.PlaySEOneShot(ClipName.SelectSE);
    }
}
