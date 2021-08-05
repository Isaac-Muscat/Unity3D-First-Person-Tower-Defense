using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayButtonPressed()
    {
        SceneManager.LoadScene("Game");
    }
}
