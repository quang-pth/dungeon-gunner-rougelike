using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class HighScoreManager : SingletonMonobehavior<HighScoreManager>
{
    private HighScore highScore = new HighScore();
    private const string saveHighScoreFilePath = "/DungeonGunnerHighScores.dat";

    protected override void Awake()
    {
        base.Awake();

        LoadScores();
    }

    private void LoadScores()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + saveHighScoreFilePath))
        {
            ClearScoreList();

            FileStream file = File.OpenRead(Application.persistentDataPath + saveHighScoreFilePath);
            highScore = (HighScore)binaryFormatter.Deserialize(file);
            file.Close();
        }
    }

    private void ClearScoreList()
    {
        highScore.scoreList.Clear();
    }

    public void AddScore(Score score, int rank)
    {
        highScore.scoreList.Insert(rank - 1, score);

        if (highScore.scoreList.Count > Settings.numberOfHighScoresToSave)
        {
            highScore.scoreList.RemoveAt(Settings.numberOfHighScoresToSave);
        }

        SaveScores();
    }

    private void SaveScores()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + saveHighScoreFilePath);

        binaryFormatter.Serialize(file, highScore);

        file.Close();
    }

    public HighScore GetHighScore()
    {
        return highScore;
    }

    // Get the highest rank of the player's score
    public int GetRank(long playerScore)
    {
        if (highScore.scoreList.Count == 0)
        { 
            return 1; 
        }

        int index = 0;

        for (int i = 0; i < highScore.scoreList.Count; i++)
        {
            index++;

            if (playerScore >= highScore.scoreList[i].playerScore)
            {
                return index;
            }
        }

        if (highScore.scoreList.Count < Settings.numberOfHighScoresToSave)
        {
            return index + 1;
        }

        return 0;
    }
}
