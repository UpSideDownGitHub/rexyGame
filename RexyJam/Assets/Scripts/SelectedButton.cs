using Dan.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SelectedButton : MonoBehaviour
{
    public bool[] selected;
    public int previous = 0;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Play":
                selected[previous] = false;
                previous = 0;
                selected[0] = true;
                break;
            case "Settings":
                selected[previous] = false;
                previous = 1;
                selected[1] = true;
                break;
            case "Quit":
                selected[previous] = false;
                previous = 2;
                selected[2] = true;
                break;
            case "Controls":
                selected[previous] = false;
                previous = 3;
                selected[3] = true;
                break;
            case "Leaderboards":
                selected[previous] = false;
                previous = 4;
                selected[4] = true;
                break;
            default:
                break;

        }
    }

    public void ConfirmPressed()
    {
        if (selected[0]) // play
        {
            SceneManager.LoadSceneAsync("SampleScene");
        }
        else if (selected[1]) // settings
        {
            print("Settings Pressed");
        }
        else if (selected[2]) // quit
        {
            print("Quit Pressed");
        }
        else if (selected[3]) // controls
        {
            print("Controls Pressed");
        }
        else if (selected[4]) // leaderboard
        {
            print("Leaderboard Pressed");
        }
    }
}
