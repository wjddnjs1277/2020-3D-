using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{

    [SerializeField] //체력
    private float hp = 100;
    public float currentHP = 0;

    [SerializeField] // 잠
    private float sp = 1000;
    public float currentSP = 0;

    [SerializeField] // 수면게이지 줄어드는 속도
    private float SleepDecreaseTime = 100;
    private float currentSleepDecreaseTime;

    [SerializeField] //방어력
    private float dp = 100;
    public float currentDP = 0;

    [SerializeField]
    private float hungry = 100;
    public float currentHungry = 0;

    [SerializeField] // 배고픔이 줄어드는 속도
    private float hungryDecreaseTime = 20;
    private float currentHungryDecreaseTime;

    [SerializeField]
    private float thirsty = 100;
    public float currentThirsty = 0;

    [SerializeField] // 목마름이 줄어드는 속도
    private float thirstyDecreaseTime = 20;
    private float currentThirstyDecreaseTime;

    [SerializeField] //만족도 즉 컨디션 
    private float satisfy = 100;
    private float currentSatisfy = 0;

    [SerializeField]//UI 이미지
    private Image[] images_Gauge;

    private const int HP = 0, DP = 1, SP = 2, HUNGRY = 3, THIRSTY = 4, SATISFY = 5; 

    // Start is called before the first frame update
    void Start()
    {
        currentHP = hp;
        currentDP = dp;
        currentHungry = hungry;
        currentSP = sp;
        currentThirsty = thirsty;
        currentSatisfy = satisfy;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Hungry();
        Thirsty();
        GaugeUpdate();
        Sleep();
        if (currentThirsty >= 50 && currentHungry >= 50)
        {
            currentHP += 0.01f;
            if (currentHP > 100)
            {
                currentHP = 100;
            }
        }
        else if(currentThirsty == 0 && currentHungry == 0)
        {
            if (currentHP > 40)
            {
                currentHP -= 0.01f;
            }
        }
        else
        {
            if (currentThirsty <= 0 || currentHungry <= 0)
            {
                if (currentHP > 70)
                {
                    currentHP -= 0.1f;
                }
            }
        }
    }

    private void Sleep()
    {
        if (currentSP > 0)
        {
            if (currentSleepDecreaseTime <= SleepDecreaseTime)
            {
                currentSleepDecreaseTime++;
            }
            else
            {
                currentSP--;
                currentSleepDecreaseTime = 0;
            }

        }
    }

    private void Hungry()
    {
        if (currentHungry > 0)
        {
            if (currentHungryDecreaseTime <= hungryDecreaseTime)
            {
                currentHungryDecreaseTime++;
            }
            else
            {
                currentHungry--;
                currentHungryDecreaseTime = 0;
            }

        }
    }

    private void Thirsty()
    {
        if (currentThirsty > 0)
        {
            if (currentThirstyDecreaseTime <= thirstyDecreaseTime)
            {
                currentThirstyDecreaseTime++;
            }
            else
            {
                currentThirsty--;
                currentThirstyDecreaseTime = 0;
            }

        }
        else
        { }
           // Debug.Log("수분섭취 수치 0됬음");
    }

    private void GaugeUpdate()
    {
        images_Gauge[HP].fillAmount = (float)currentHP / hp;
        images_Gauge[DP].fillAmount = (float)currentDP / dp;
        images_Gauge[SP].fillAmount = (float)currentSP / sp;
        images_Gauge[HUNGRY].fillAmount = (float)currentHungry / hungry;
        images_Gauge[THIRSTY].fillAmount = (float)currentThirsty / thirsty;
        images_Gauge[SATISFY].fillAmount = (float)currentSatisfy / satisfy;

    }
    //체력 및 방어력? 증가 감소

    public void IncreaseHP(int _count)
    {
        if (currentHP + _count < hp)
            currentHP += _count;
        else
            currentHP = hp;
    }

    public void DecreseHP(int _count)
    {
        if(currentDP >0)
        {
            DecreseDP(_count);
            return;
        }

        currentHP -= _count;
        if (currentHP <= 0)
            Debug.Log("체력이 0입니다.");
    }

    public void IncreaseDP(int _count)
    {
        if (currentDP + _count < dp)
            currentDP += _count;
        else
            currentDP = dp;
    }
    public void DecreseDP(int _count)
    {
        currentDP -= _count;
        if (currentDP <= 0)
            Debug.Log("방어력이 0입니다.");
    }

    public void IncreaseHungry(int _count)
    {
        if (currentHungry + _count < hungry)
            currentHungry += _count;
        else
            currentHungry = hungry;
    }
    public void DecreseHungry(int _count)
    {
        currentHungry -= _count;
    }

}
