using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalSave
{
    public static void SaveCatchHighscore(int amount)
    {
        PlayerPrefs.SetInt("MinigameCatch", amount);
    }
    public static int GetCatchHighScore()
    {
        return PlayerPrefs.GetInt("MinigameCatch", 0);
    }
}
