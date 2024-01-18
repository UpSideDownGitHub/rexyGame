using UnityEngine;
using TMPro;
using Dan.Main;
using Dan;
using Dan.Models;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LeaderboardOnly : MonoBehaviour
{
    [Header("Leaderboard")]
    public TMP_Text[] playerInfo;
    public TMP_Text previousScoreText;
    public bool hasPrevious;
    public int previousScore;
    public int score;

    private void Start()
    {
        LoadEntries();
        GetPersonalEntry();
    }

    private void LoadEntries()
    {
        Leaderboards.RexyGameJamLeaderboard.GetEntries(entries =>
        {
            foreach (var t in playerInfo)
                t.text = "";
            var length = Mathf.Min(playerInfo.Length, entries.Length);
            for (int i = 0; i < length; i++)
                playerInfo[i].text = $"{entries[i].Rank}. {entries[i].Username} - {entries[i].Score}";
        });
    }

    public void GetPersonalEntry()
    {
        Leaderboards.RexyGameJamLeaderboard.GetPersonalEntry(OnPersonalEntryLoaded, errored =>
        {
            print("Error getting peronal");
        });
    }

    private void OnPersonalEntryLoaded(Entry entry)
    {
        if (!entry.Username.Equals("Unknown"))
        {
            hasPrevious = true;
            previousScore = entry.Score;
            previousScoreText.text = $"{entry.RankSuffix()}. {entry.Username} : {entry.Score}";
        }
        else
        {
            hasPrevious = false;
            previousScoreText.text = "No Previous";
        }

    }
}
