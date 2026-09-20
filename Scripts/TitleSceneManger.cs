using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManger : MonoBehaviour
{
    private int _highScore = 0;

    [SerializeField] private Button _startButton;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private RectTransform _titleTextOP;    //タイトルテキスト「ワンオペ」
    [SerializeField] private RectTransform _titleTextFS;    //タイトルテキスト「ファッションショー」

    //テキストの最終位置
    private Vector2 _finalTextOPPos;  
    private Vector2 _finalTextFSPos;

    static public TitleSceneManger Instance {  get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //ハイスコアの取得
        _highScore = PlayerPrefs.GetInt("Score0", 0);

        //テキストの最終位置
        _finalTextOPPos = _titleTextOP.anchoredPosition;
        _finalTextFSPos = _titleTextFS.anchoredPosition;

        //ボタンは最初非表示に
        _startButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        //ハイスコアの表示
        _highScoreText.text = "HIGH SCORE : " + _highScore.ToString();

        //タイトル演出未再生の時、再生
        if (!GameSceneManager.Instance.PlayedTitleDirection)
            StartTitleDirection();
        //2回目以降の時は再生しない
        else
        {
            _startButton.gameObject.SetActive(true);
        }

        //タイトルBGM
        AudioManager.Instance.PlayBGM(ClipName.TitleBGM);
    }

    //タイトル演出スタート
    private void StartTitleDirection()
    {
        //game scene managerに演出再生を通知
        GameSceneManager.Instance.OnStartingTitleDirection();

        //タイトルテキストを最初は上に置いておく
        _titleTextOP.anchoredPosition += Vector2.up * 1000f;
        _titleTextFS.anchoredPosition += Vector2.up * 2000f;

        Sequence sequence = DOTween.Sequence();

        // 「ファッションショー」が落下
        sequence.Append(
            _titleTextFS.DOAnchorPos(_finalTextFSPos, 1.5f).SetEase(Ease.OutBounce)
        );

        //0.3s待つ
        sequence.AppendInterval(0.3f);

        //「ワンオペ」が回転しながら落下
        sequence.Append(
            _titleTextOP.DOAnchorPos(_finalTextOPPos, 0.5f).SetEase(Ease.InQuad)
            );
        sequence.Join(
            _titleTextOP.DORotate(new Vector3(0,0,1800f),0.5f,RotateMode.FastBeyond360)
            );

        //「ファッションショー」が少し沈んで戻る
        Vector2 originalFSPos = _finalTextFSPos;
        sequence.Append(_titleTextFS
            .DOAnchorPos(originalFSPos + Vector2.down * 70f, 0.1f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad)
            );

        //「ワンオペ」も
        Vector2 originalOPPos = _finalTextOPPos;
        sequence.Join(_titleTextOP
            .DOAnchorPos(originalOPPos + Vector2.down * 70f, 0.1f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad)
            );

        //0.3s待つ
        sequence.AppendInterval(0.3f);

        //タイトルボタンを表示する
        sequence.AppendCallback(() =>
        {
            _startButton.gameObject.SetActive(true);
        });
    }

}
