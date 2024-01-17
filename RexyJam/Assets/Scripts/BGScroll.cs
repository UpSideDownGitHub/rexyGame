using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGScroll : MonoBehaviour
{
    public RawImage image;
    public Rigidbody2D player;
    public float speed;

    public void Update()
    {
        image.uvRect = new Rect(image.uvRect.position + player.velocity * Time.deltaTime * speed, image.uvRect.size);
    }
}