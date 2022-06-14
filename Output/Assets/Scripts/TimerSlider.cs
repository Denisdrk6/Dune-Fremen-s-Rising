using System;
using RagnarEngine;

public class TimerSlider : RagnarComponent
{
    float TotalTime = 0;
    Vector3 newBounds;
    Vector3 Newposition;
    GameObject enemy=null;
    float lastX = 100;
    int typeOfEnemy=0;
    string theAction;
    public void Start()
    {
        Newposition = new Vector3(0, 0, 0);
        newBounds = new Vector3(0, 0, 0);
        gameObject.GetComponent<UIImage>().SetImageGeneralColor(255, 0, 0);
        gameObject.childs[0].GetComponent<UIImage>().SetImageGeneralColor(0, 0, 0);
        newBounds.Set(100, 7, 0);
        gameObject.childs[0].GetComponent<Transform2D>().SetSize(newBounds);
    }
    void SetTimer(float actualTime,Vector3 position)
    {
        float mult = actualTime / TotalTime;
        float newBound = mult * 100;

        float x=lastX - newBound;
        newBounds.Set(newBound, 7, 0);

        //gameObject.GetComponent<Transform>() = position;
        Newposition = Camera.WorldToScreen(position);
        Newposition.y= Newposition.y + 100;
        Newposition.x = Newposition.x-(x/2);
        gameObject.GetComponent<Transform2D>().position2D= Newposition;
        gameObject.GetComponent<Transform2D>().SetSize(newBounds);
        Newposition.y = Newposition.y + 20;
        Newposition.x = Newposition.x + (x / 2);
        gameObject.childs[0].GetComponent<Transform2D>().position2D = Newposition;
    }
    public void getGa(GameObject obj,float time,int enemyTy,string action)
    {
        enemy = obj;
        typeOfEnemy = enemyTy;
        TotalTime = time;
        theAction=action;
    }

    public void Update()
    {
        if (enemy != null)
        {
            float time=0;
            switch (typeOfEnemy)
            {
                case -1:
                    if (theAction == "distractedTimer")
                    {
                        time = enemy.GetComponent<EnemyBoss>().distractedTimer;
                        SetTimer(time, enemy.GetComponent<Transform>().globalPosition);

                    }
                    else if (theAction == "stunnedTimer")
                    {
                        time = enemy.GetComponent<EnemyBoss>().stunnedTimer;
                        Newposition = enemy.GetComponent<Transform>().globalPosition;
                        
                        SetTimer(time, Newposition);
                    }
                    else if (theAction == "controlledCooldown")
                    {
                        time = enemy.GetComponent<EnemyBoss>().controlledCooldown;
                        SetTimer(time, enemy.GetComponent<Transform>().globalPosition);
                    }
                    if (enemy.GetComponent<EnemyBoss>().state == EnemyState.DEATH) time = 0;
                    break;

                case (int)EnemyType.BASIC:
                    if(theAction== "distractedTimer")
                    {
                        time = enemy.GetComponent<BasicEnemy>().distractedTimer;
                        SetTimer(time, enemy.GetComponent<Transform>().globalPosition);

                    }else if(theAction == "stunnedTimer")
                    {
                        time = enemy.GetComponent<BasicEnemy>().stunnedTimer;
                        SetTimer(time, enemy.GetComponent<Transform>().globalPosition);
                    }
                    else if(theAction == "controlledCooldown")
                    {
                        time = enemy.GetComponent<BasicEnemy>().controlledCooldown;
                        SetTimer(time, enemy.GetComponent<Transform>().globalPosition);
                    }
                    if (enemy.GetComponent<BasicEnemy>().state == EnemyState.DEATH) time = 0;
                    break;
                case (int)EnemyType.TANK:
                    if (theAction == "distractedTimer")
                    {
                        time = enemy.GetComponent<TankEnemy>().distractedTimer;
                        SetTimer(time, enemy.GetComponent<Transform>().globalPosition);
                    }
                    else if (theAction == "stunnedTimer")
                    {
                        time = enemy.GetComponent<TankEnemy>().stunnedTimer;
                        Newposition = enemy.GetComponent<Transform>().globalPosition;
                        
                        SetTimer(time, Newposition);
                    }
                    else if (theAction == "controlledCooldown")
                    {
                        time = enemy.GetComponent<TankEnemy>().controlledCooldown;
                        SetTimer(time, enemy.GetComponent<Transform>().globalPosition);
                    }
                    if (enemy.GetComponent<TankEnemy>().state == EnemyState.DEATH) time = 0;
                    break;
                case (int)EnemyType.UNDISTRACTABLE:
                    if (theAction == "distractedTimer")
                    {
                        time = enemy.GetComponent<UndistractableEnemy>().distractedTimer;
                        SetTimer(time, enemy.GetComponent<Transform>().globalPosition);

                    }
                    else if (theAction == "stunnedTimer")
                    {
                        time = enemy.GetComponent<UndistractableEnemy>().stunnedTimer;
                       
                        Newposition = enemy.GetComponent<Transform>().globalPosition;
                        
                        SetTimer(time, Newposition);
                    }
                    else if (theAction == "controlledCooldown")
                    {
                        time = enemy.GetComponent<UndistractableEnemy>().controlledCooldown;
                        
                        SetTimer(time, enemy.GetComponent<Transform>().globalPosition);
                    }
                    if (enemy.GetComponent<UndistractableEnemy>().state==EnemyState.DEATH) time = 0;
                    break;
                

            }
            if (time <= 0)
            {
                InternalCalls.Destroy(gameObject);
            }
        }
        
    }

}