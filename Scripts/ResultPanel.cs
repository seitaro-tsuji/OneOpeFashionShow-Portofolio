using TMPro;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{
    [Header("Score Showing")]
    [SerializeField] private TextMeshProUGUI _orderCountText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _totalScoreText;
    [SerializeField] private TextMeshProUGUI _highScoreText;

    private void Start()
    {
        //リザルト画面のスコアテキスト更新
        //SetScoreTexts();
    }

    public void SetScoreTexts(int orderCount, int money, int totalScore, int highScore)
    {
        _orderCountText.text = "達成オーダー  " + orderCount.ToString();
        _moneyText.text = "所持金  " + money.ToString();
        _totalScoreText.text = "SCORE " +  totalScore.ToString();
        _highScoreText.text = "HIGH SCORE  " + highScore.ToString();
    }
}
