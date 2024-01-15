using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SelectedButton : MonoBehaviour
{
    public bool play, settings, quit, controls, leaderboards;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Play"))
        {
            play = true;
            settings = false;
            quit = false;
            controls = false;
            leaderboards = false;

        }

        else if (collision.tag == ("Settings"))
        {
            settings = true;
            play = false;
            quit = false;
            controls = false;
            leaderboards = false;
        }

        else if (collision.tag == ("Quit"))
        {
            quit = true;
            play = false;
            settings = false;
            controls = false;
            leaderboards = false;
        }

        else if (collision.tag == ("Controls"))
        {
            controls = true;
            play = false;
            settings = false;
            quit = false;
            leaderboards = false;
        }

        else if (collision.tag == ("Leaderboards"))
        {
            leaderboards = true;
            play = false;
            settings = false;
            quit = false;
            controls = false;

        }

    }
    public void OnSelect()
    {
        if (play == true)
        {
            SceneManager.LoadScene("SampleScene");
        }

        else if (settings == true)
        {

        }

        else if (quit == true)
        {

        }

        else if (controls == true)
        {

        }

        else if (leaderboards == true)
        {

        }
    }
}
