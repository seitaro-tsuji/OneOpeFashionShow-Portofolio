using System.Collections.Generic;
using UnityEngine;

public class ResultSceneManager : MonoBehaviour
{
    private int _orderCount = 0;
    private int _money = 0;
    private int _totalScore = 0;
    private int _rank = 0; //ランキング順位
    private int _rankingCount = 5;  //ランキング上位いくつ保存するか
    private List<int> _scoreRanking = new List<int>();

    [SerializeField] private ResultPanel _resultPanel;

    static public ResultSceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //スコアの取得
        _orderCount = GameSceneManager.Instance.SuccessOrderCount;
        _money = GameSceneManager.Instance.Money;
        _totalScore = GameSceneManager.Instance.TotalScore;

        //ランキングの取得
        GetScoreRanking();

        //ランキングへの登録
        AddScoreRanking(_totalScore);

        //テキストの更新
        _resultPanel.SetScoreTexts(_orderCount, _money, _totalScore, _scoreRanking[0]);
    }

    private void Start()
    {
        //BGM再生
        AudioManager.Instance.PlayBGM(ClipName.ResultBGM);
    }

    public void GetScoreRanking()
    {
        for (int i = 0; i < _rankingCount; i++)
        {
            _scoreRanking.Add(PlayerPrefs.GetInt("Score" + i, 0));
        }
    }

    //ランキングに今回のスコアを登録する
    public void AddScoreRanking(int score)
    {
        //今回の順位を取得
        _rank = 0;
        for (int i = 0; i < _rankingCount; i++)
        {
            if (score > _scoreRanking[i])
            {
                break;
            }
            _rank++;
        }

        //ランク外の時
        if (_rank >= _rankingCount)
        {
            return;
        }

        //ランクインしたとき
        _scoreRanking.Insert(_rank, score);
        _scoreRanking.RemoveAt(_rankingCount);

        //登録
        for (int i = 0; i < _rankingCount; i++)
        {
            PlayerPrefs.SetInt("Score" + i, _scoreRanking[i]);
        }
        PlayerPrefs.Save();
    }
}
