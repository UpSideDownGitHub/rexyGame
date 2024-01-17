using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public int powerupID;

    // public variable
    public float lerpTime = 0.01f;
    public float attactDistance;
    public bool attracted = false;
    public float time;


    // private variables 
    private GameObject _player;


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // this will make the pickup be attracted to the player
    public void freePickup()
    {
        attracted = true;
    }

    // Update is called once per frame
    void Update()
    {
        // calculate the distance to the player
        float distX = Mathf.Abs(transform.position.x - _player.transform.position.x);
        float distY = Mathf.Abs(transform.position.y - _player.transform.position.y);

        // is not currently being attracted
        if (!attracted)
        {
            // if within range
            if (Vector2.Distance(transform.position, _player.transform.position) < attactDistance)
            {
                // go to the player
                attracted = true;
                time = 0;
            }
            return;
        }

        // lerp the position of pickup towards the player (accounting for the distance (higher speed higher distance))
        time += Mathf.SmoothStep(0.0f, 1.0f, Time.deltaTime / lerpTime);
        transform.position = Vector2.Lerp(transform.position, _player.transform.position, time);
    }
}
