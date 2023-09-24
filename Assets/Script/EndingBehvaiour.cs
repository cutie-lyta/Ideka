using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingBehvaiour : MonoBehaviour
{
    public void ReturnToTitleScreen()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
