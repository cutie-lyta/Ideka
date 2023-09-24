using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleBehaviour : MonoBehaviour
{
    public int default_number;
    public GameObject obj;

    TextMeshProUGUI tmp;

    int min = 4;
    int max = 8;

    // Start is called before the first frame update
    void Start()
    {
        // Framerate
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        // Recupere le TextMeshPro de obj des le debut
        tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.text = default_number.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Increment player number
    public void ButtonPressed(bool plus)
    {
        // Je le recupere
        int number = int.Parse(tmp.text);

        // Je le modifie
        if (plus)
        {
            number = number == max ? min : number + 1;
        }
        else
        {
            number = number == min ? max : number - 1;
        }

        // Je le remets
        tmp.text = number.ToString();
    }

    public void Play()
    {
        // Set le nombre de player, et commence la partie
        int number = int.Parse(tmp.text);
        Parameters.player_number = number;
        SceneManager.LoadScene("RedistributionScene");
    }
}
