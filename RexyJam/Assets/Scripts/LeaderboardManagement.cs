using UnityEngine;
using TMPro;
using Dan.Main;
using Dan;
using Dan.Models;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LeaderboardManagement : MonoBehaviour
{
    [Header("Keyboard")]
    public GameObject Cog;
    public string[] alphabet = new string[26] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    public string currentName;
    public TMP_Text[] letters;
    public GameObject[] selectionIndicators;
    public TMP_Text currentScore;

    public float rotationSpeed;
    private float _lookVec;
    private float _lookVecRex;

    public void OnLeftStickRexy(InputAction.CallbackContext ctx) => _lookVecRex = ctx.ReadValue<float>();
    public void OnLeftStickOther(InputAction.CallbackContext ctx) => _lookVec = ctx.ReadValue<float>();
    public void OnDelete(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPerformedThisFrame())
            DeletePressed();
    }
    public void OnContinue(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPerformedThisFrame())
            ContinuePressed();

    }
    public void OnSelect(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPerformedThisFrame())
            SelectPressed();
    }
    public void OnSelect2(InputAction.CallbackContext ctx)
    {
        if (ctx.action.WasPerformedThisFrame())
            MainMenuPressed();
    }

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
        currentScore.text = PlayerPrefs.GetInt("Score", 0).ToString();
        score = PlayerPrefs.GetInt("Score", 0);
    }

    void Update()
    {
        if (_lookVecRex != 0)
        {
            var val = _lookVecRex;
            if (_lookVecRex < 0)
                val = -(1 - Mathf.Abs(_lookVecRex));
            else
                val = 1 - val;
            Cog.transform.Rotate(new Vector3(0, 0, -rotationSpeed * val));
        }
        else
        {
            Cog.transform.Rotate(new Vector3(0, 0, -rotationSpeed * _lookVec));
        }

    }

    public void DisableIndicators()
    {
        selectionIndicators[0].SetActive(false);
        selectionIndicators[1].SetActive(false);
        selectionIndicators[2].SetActive(false);
    }

    public void DeletePressed()
    {
        if (currentName.Equals(""))
            return;
        DisableIndicators();
        letters[currentName.Length - 1].text = "";
        selectionIndicators[currentName.Length - 1].SetActive(true);
        currentName = currentName.Remove(currentName.Length - 1);
        return;
    }

    public void MainMenuPressed()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void ContinuePressed()
    {
        // dont upload if not higher or no name
        if (currentName.Equals(""))
            return;
        if (hasPrevious)
        {
            if (previousScore > score)
                return;
        }
        UploadEntry();
    }

    public void SelectPressed()
    {
        // rotation per == 360/26
        // area for each is 
        // (360/26 * i) - 360/52 <-> (360/26 * i) + 360/52   
        // to find the letter get current rotation
        for (int i = 0; i < 26; i++)
        {
            if (Cog.transform.rotation.eulerAngles.z >= (360f / 26f * i) - 360f / 52f &&
                Cog.transform.rotation.eulerAngles.z <= (360f / 26f * i) + 360f / 52f)
            {
                if (currentName.Length > 2)
                    return;
                currentName += alphabet[i];
                letters[currentName.Length - 1].text = alphabet[i];
                DisableIndicators();
                if (currentName.Length > 2)
                    return;
                selectionIndicators[currentName.Length].SetActive(true);
                return;
            }
        }
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
    public void UploadEntry()
    {
        //If you clear PlayerPrefs, you will be able to submit another score, now as a different player.
        // sneaky to clear player prefs, then set the assist after to save it
        Leaderboards.RexyGameJamLeaderboard.UploadNewEntry(currentName, score, isSuccessful =>
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
