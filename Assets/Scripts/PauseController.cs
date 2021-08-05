using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Taken from https://gamedevbeginner.com/the-right-way-to-pause-the-game-in-unity/
public class PauseController : MonoBehaviour
{
    public GameObject shop, info;
    public GameObject player;
    public static bool gameIsPaused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameIsPaused = !gameIsPaused;
            PauseGame();
        }
        if(Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void PauseGame ()
    {
        if(gameIsPaused)
        {
            info.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else 
        {
            info.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }

    public void HandleInput(int val){
            if(val == 0){
                player.GetComponent<Player>().selectedTower = "Basic Turret";
            }
            if(val == 1){
                player.GetComponent<Player>().selectedTower = "Splash Turret";
            }
    }
}
