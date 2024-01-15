using UnityEngine;
using TMPro;

// NOTE: Make sure to include the following namespace wherever you want to access Leaderboard Creator methods
using Dan.Main;
using Dan;
using UnityEngine.SocialPlatforms.Impl;
using Dan.Models;

public class LeaderboardManagement : MonoBehaviour
{
    public TMP_Text[] entryTextObjects;
    public TMP_Text previousScoreText;
    public TMP_InputField usernameInputField;

    private void Start()
    {
        LoadEntries();
        GetPersonalEntry();
    }

    private void LoadEntries()
    {
        Leaderboards.RexyGameJamLeaderboard.GetEntries(entries =>
        {
            foreach (var t in entryTextObjects)
                t.text = "";
            var length = Mathf.Min(entryTextObjects.Length, entries.Length);
            for (int i = 0; i < length; i++)
                entryTextObjects[i].text = $"{entries[i].Rank}. {entries[i].Username} - {entries[i].Score}";
        });
    }
    public void UploadEntry()
    {
        int score = PlayerPrefs.GetInt("Score", 100);
        Leaderboards.RexyGameJamLeaderboard.UploadNewEntry(usernameInputField.text, score, isSuccessful =>
        {
            if (isSuccessful)
                LoadEntries();
        }, errored =>
        {
            print("Error Uploading");
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
        previousScoreText.text = $"{entry.RankSuffix()}. {entry.Username} : {entry.Score}";
    }
}
