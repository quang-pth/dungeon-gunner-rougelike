using UnityEngine;

public class DisplayHighScoreUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10), Header("OBJECT REFERENCES")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the child Content gameobject Transform component")]
    #endregion
    [SerializeField] private Transform contentAnchorTransform;

    private void Start()
    {
        HighScore highScore = HighScoreManager.Instance.GetHighScore();

        int rank = 0;
        foreach(Score score in highScore.scoreList)
        {
            rank++;

            GameObject scoreGameObject = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);
            ScorePrefab scorePrefab = scoreGameObject.GetComponent<ScorePrefab>();

            scorePrefab.rankTMP.text = rank.ToString();
            scorePrefab.nameTMP.text = score.playerName;
            scorePrefab.levelTMP.text = score.levelDescription;
            scorePrefab.scoreTMP.text = score.playerScore.ToString("###,###0");
        }

        // Add blank line at the end
        Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);
    }
}
