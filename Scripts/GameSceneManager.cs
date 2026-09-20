using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//シーン遷移の管理をするシングルトン　常駐
public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private BlackOutScreen _blackOutScreen;
    [SerializeField] private GameObject _disableControllScreen; //画面操作を不可能にするための透明画像
    [SerializeField] private RectTransform _startTextTrans;        //インゲーム開始演出用テキスト"Start!!"
    [SerializeField] private RectTransform _endTextTrans;           //インゲーム終了時演出テキスト"FINISH"
    private TextMeshProUGUI _startText; //テキスト
    private TextMeshProUGUI _endText; //テキスト

    //演出の初期条件
    private Vector2 _startTextInitialPos;
    private Vector3 _startTextInitialScale;
    private Color _startTextInitialColor;

    private Vector2 _endTextInitialPos;
    private Vector3 _endTextInitialScale;
    private Color _endTextInitialColor;


    //タイトル演出を再生したかどうか
    private bool _playedTitleDirection = false;
    public bool PlayedTitleDirection => _playedTitleDirection;

    public string PrevSceneName;

    //リザルトでのスコアの一次保存用
    private int _successOrderCount=0;
    private int _money=0;
    private int _totalScore=0;

    //リザルト表示時に各種スコアを取得する
    public int SuccessOrderCount => _successOrderCount;
    public int Money => _money;
    public int TotalScore => _totalScore;


    static public GameSceneManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        //Start,Endテキスト
        _startText = _startTextTrans.GetComponent<TextMeshProUGUI>();
        _endText = _endTextTrans.GetComponent<TextMeshProUGUI>();

        //初期条件の保存
        _startTextInitialPos = _startText.rectTransform.anchoredPosition;
        _startTextInitialScale = _startText.rectTransform.localScale;
        _startTextInitialColor = _startText.color;

        _endTextInitialPos = _endText.rectTransform.anchoredPosition;
        _endTextInitialScale = _endText.rectTransform.localScale;
        _endTextInitialColor = _endText.color;
    }

    public void ChangeScene(string nextSceneName)
    {
        /*
        if (nextSceneName != "InGameTest" && nextSceneName != "InGameTestCopy")
        {
            Debug.Log("シーン遷移:ingame以外");
            ChangeSceneAsync(nextSceneName).Forget();
        }
        else
        {
            Debug.Log("インゲームへの遷移");
            ChangeSceneToIngameAsync().Forget();
        }
        */

        PrevSceneName = SceneManager.GetActiveScene().name;
        ChangeSceneAsync(nextSceneName).Forget();
    }

    //シーン遷移用async
    private async UniTask ChangeSceneAsync(string nextSceneName)
    {
        //操作不可能にする
        DisableControll();

        //フェードアウト
        await _blackOutScreen.FadeOutAsync(duration: 0.3f);

        //BGMきってシーン遷移
        AudioManager.Instance.StopBGM();
        SceneManager.LoadScene(nextSceneName);
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

        //フェードイン
        await _blackOutScreen.FadeInAsync(duration: 0.3f);

        //操作可能にする
        EnableControll();
    }

    //インゲームに移行したときのシーン遷移用
    private async UniTask ChangeSceneToIngameAsync()
    {
        await ChangeSceneAsync("InGameTestCopy");   //注意：完成版ではInGameTestにする

        //操作不可能にする
        DisableControll();

        //todo:インゲーム開始演出
        await InGameDirection();

        //操作可能にする
        EnableControll();
    }

    //タイトル演出開始時に呼ぶ(演出再生済みにするよう)
    public void OnStartingTitleDirection()
    {
        _playedTitleDirection = true;
    }

    //インゲーム開始時の演出
    public async UniTask InGameDirection()
    {
        //ここにチュートリアル表示→クリック/タップで消すという手順を入れるかも

        //先に初期状態にテキストを戻しておく
        _startTextTrans.anchoredPosition = _startTextInitialPos;
        _startTextTrans.localScale = _startTextInitialScale;
        _startText.color = _startTextInitialColor;

        //START!!の文字を演出として表示し、その後操作可能にする
        //初期位置と最終位置の設定(真ん中)
        _startTextTrans.gameObject.SetActive(true);
        Vector2 finalTextPos = _startTextTrans.anchoredPosition;
        _startTextTrans.anchoredPosition += Vector2.up * 1400f;

        //操作不可能にする
        DisableControll();

        //-----------------------------------------演出-------------------------------------------------------
        Sequence sequence = DOTween.Sequence();

        //0.4秒待つ(シーン遷移のフェードアウトがあるから
        sequence.AppendInterval(0.4f);

        //テキストの落下
        sequence.Append(_startTextTrans.DOAnchorPos(finalTextPos, duration: 0.5f).SetEase(Ease.OutBounce));

        //ちょっと待つ
        sequence.AppendInterval(0.3f);

        //拡大しながらフェード
        sequence.Append(_startTextTrans.DOScale(5f, duration: 0.5f).SetEase(Ease.OutQuad));
        sequence.Join(_startText.DOFade(0, duration: 0.5f).SetEase(Ease.OutQuad));
        sequence.JoinCallback(() => AudioManager.Instance.PlaySEOneShot(ClipName.GameStartSE));

        //
        sequence.AppendInterval(0.5f);
        sequence.AppendCallback(() => AudioManager.Instance.PlayBGM(ClipName.InGameBGM));

        //--------------------------------------------演出---------------------------------------------------------

        //DOTweenの演出終了まで待つ
        await sequence.AsyncWaitForCompletion();

        //操作可能にする
        EnableControll();
        _startTextTrans.gameObject.SetActive(false);
    }

    //インゲーム終了時の演出
    public async UniTask InGameFinishDirection()
    {
        //先に初期状態にテキストを戻しておく
        _endTextTrans.anchoredPosition = _endTextInitialPos;
        _endTextTrans.localScale = _endTextInitialScale;
        _endText.color = _endTextInitialColor;

        //FINISHの文字を演出として表示し、その後操作可能にする
        //初期位置と最終位置の設定(真ん中)
        _endTextTrans.gameObject.SetActive(true);
        Vector2 finalTextPos = _endTextTrans.anchoredPosition;
        _endTextTrans.anchoredPosition += Vector2.up * 1400f;

        //操作不可能にする
        DisableControll();

        //-----------------------------------------演出-------------------------------------------------------

        Sequence sequence = DOTween.Sequence();

        //テキストの落下
        sequence.Append(_endTextTrans.DOAnchorPos(finalTextPos, duration: 0.5f).SetEase(Ease.OutBounce));
        //同時にSEを流してBGM切る
        sequence.JoinCallback(() => AudioManager.Instance.PlaySEOneShot(ClipName.GameFinishSE));
        sequence.JoinCallback(() => AudioManager.Instance.StopBGM());

        //ちょっと待つ
        sequence.AppendInterval(0.3f);

        //拡大しながらフェード
        sequence.Append(_endTextTrans.DOScale(5f, duration: 0.5f).SetEase(Ease.OutQuad));
        sequence.Join(_endText.DOFade(0, duration: 0.5f).SetEase(Ease.OutQuad));

        //-----------------------------------------演出-------------------------------------------------------

        //DOTweenの演出終了まで待つ
        await sequence.AsyncWaitForCompletion();

        //操作可能にする
        EnableControll();
        _endTextTrans.gameObject.SetActive(false);
    }

    //ゲーム終了時に各種スコアの設定をする用
    public void SetResultScore(int successOrderCount, int money, int score)
    {
        _successOrderCount = successOrderCount;
        _money = money;
        _totalScore = score;
    }

    //画面の操作可能/不可能を切り替える
    public void EnableControll()=>_disableControllScreen?.SetActive(false);
    public void DisableControll()=>_disableControllScreen?.SetActive(true);
}
