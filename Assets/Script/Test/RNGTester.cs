using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNGTester : MonoBehaviour
{
    public GameObject dice;

    public List<int> num_of_rolls;

    public GameObject[] bars = new GameObject[6];
    public GameObject Canvas;

    public bool direct;
    public bool rigged;

    int timer = -1;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 1920;

        for (int i = 0; i < 6; i++) num_of_rolls.Add(0);
        if (direct) timer = UnityEngine.Random.Range(0, 20000);
    }

    void Reciever()
    {
        if (!direct) StartCoroutine(generateDistribution(100));
    }

    // Update is called once per frame
    void Update()
    {
        if(timer == 0) { 
            StartCoroutine(generateDistribution(100));
        }
        if(timer != -1)
        {
            timer--;
        }
    }

    IEnumerator generateDistribution(int num_of_test)
    {
        dice.GetComponent<DiceBehvaiour>().rigged = rigged;

        Canvas.GetComponent<CanvasGroup>().alpha = 0;
        for(int i = 0; i < num_of_test; i++)
        {
            StartCoroutine(launchOneDie((value) => {
                try
                {
                    num_of_rolls[value - 1]++;
                }
                catch (Exception e)
                {
                    print(value - 1);
                }
            }));
            yield return new WaitForSeconds((0.25f + 0.375f) / 4.0f);
        }

        for (int index = 0; index < 6; index++)
        {

            // Get percentage
            // Produit en croix
            // -------------
            // | val | num |
            // -------------  = val * 100 / numm
            // |  ?  | 100 |
            // -------------
            int percentage = num_of_rolls[index] * 100 / num_of_test;


            // taille Y max des batons : 4.5 (mini = 0)
            // Produit en croix
            // -------------
            // | per | 100 |
            // -------------  = per * 4.5 / 100
            // |  ?  | 4.5 |
            // -------------
            float size = percentage * 4.5f / 100;


            bars[index].transform.localScale = new Vector3(bars[index].transform.localScale.x, size, bars[index].transform.localScale.z);

            // Position Y des batons
            // Entre -2.05 (0%)
            // Et 0.2 (100%)
            //
            // Car taille relative au milieu, et non extremité
            //
            // La difference entre 0.2 et -2.05 = 1.85
            // Encore un produit en croix
            // -------------
            // | per | 100 |
            // -------------  = per * 1.85 / 100
            // |  ?  | 1.85| (1.85 de PLUS comparer a 0)
            // -------------
            //
            // Ce produit en croix trouve combien on dois additionner a -2.05 pour obtenir la bonne position

            float position = (-2.05f) + (percentage * 1.85f / 100);

            bars[index].transform.localPosition = new Vector3(bars[index].transform.localPosition.x, position, bars[index].transform.localPosition.z);

            print((index + 1).ToString() + " : " + percentage.ToString());

        }

        Canvas.GetComponent<CanvasGroup>().alpha = 1;

        if(direct) BroadcastMessage("Reciever");

        yield return null;
    }

    IEnumerator launchOneDie(System.Action<int> callback)
    {
        int result;

        dice.GetComponent<DiceBehvaiour>().LaunchDice(true);

        yield return new WaitForSeconds(0.25f / 4.0f);
        result = (int)dice.GetComponent<DiceBehvaiour>().latestRoll;

        yield return new WaitForSeconds(0.375f / 4.0f);

        // Safe Wait
        yield return new WaitForEndOfFrame();

        callback(result);
    }
}
