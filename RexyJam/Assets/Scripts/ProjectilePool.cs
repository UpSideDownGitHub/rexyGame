using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public int[] maxProj;
    public List<Proj> proj = new List<Proj>();
    public GameObject[] projPrefabs;

    public static ProjectilePool instance;

    // called when the object is being loaded 
    public void Awake()
    {
        instance = this;

        // this will spawn maxProj amount of each pickup and store them in the proj list
        for (int j = 0; j < projPrefabs.Length; j++)
        {
            // for the amount of projectiles
            for (int i = 0; i < maxProj[j]; i++)
            {
                proj.Add(Instantiate(projPrefabs[j], new Vector3(0, 0, 0), new Quaternion()).GetComponent<Proj>());
            }
        }

        // turn off all of the proj
        for (int i = 0; i < proj.Count; i++)
        {
            proj[i].disable();
        }
    }

    public GameObject SpawnProjectile(int projID, Vector2 spawnPos, Quaternion rot)
    {
        int minPos = 0;
        for (int i = 0; i < projID; i++)
            minPos += maxProj[i];

        for (int i = minPos; i < minPos + maxProj[projID]; i++)
        {
            if (proj[i].isSpawnable)
            {
                proj[i].setPosition(spawnPos);
                proj[i].setRotation(rot);
                proj[i].enable();
                return proj[i].gameObject;
            }
        }
        return null; 
    }
}