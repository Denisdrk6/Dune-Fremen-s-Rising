using System;
using RagnarEngine;

public class Barrel : RagnarComponent
{

    GameObject boss;
    public int barrelIndex = 0;

    float deathTimer = -1f;

    // Use this for initialization
    public void Start()
    {
        boss = GameObject.Find("Boss");

        for (int i = 0; i < gameObject.childs.Length; ++i)
        {
            if (gameObject.childs[i].name == "BarrelExplosion")
            {
                gameObject.childs[i].GetComponent<ParticleSystem>().Pause();
                break;
            }
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKey(KeyCode.H) == KeyState.KEY_DOWN)
        {
            NotifyBoss();
        }

        if (deathTimer >= 0)
        {
            deathTimer -= Time.deltaTime;
            if (deathTimer < 0)
            {
                deathTimer = -1f;
                InternalCalls.Destroy(gameObject);
            }
        }
    }

    public void OnColision(Rigidbody other)
    {
        if (other.gameObject.name == "Knife")
        {
            if (boss.transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude < 2.0f)
            {
                boss.GetComponent<Boss>().stunnedHits++;
            }
            boss.GetComponent<Boss>().barrelCount--;
            boss.GetComponent<Boss>().barrelLocations[barrelIndex].isDestroyed = true;
            for (int i = 0; i < gameObject.childs.Length; ++i)
            {
                if (gameObject.childs[i].name == "BarrelExplosion")
                {
                    gameObject.childs[i].GetComponent<ParticleSystem>().Play();
                    break;
                }
            }
            deathTimer = 0.4f;
        }
    }
    void NotifyBoss()
    {
        Debug.Log("Destroying barrel ----------------------------------------------------------------------");
        boss.GetComponent<Boss>().stunnedHits++;
        boss.GetComponent<Boss>().barrelLocations[barrelIndex].isDestroyed = true;
        boss.GetComponent<Boss>().barrelCount--;
        InternalCalls.Destroy(gameObject);
    }
}