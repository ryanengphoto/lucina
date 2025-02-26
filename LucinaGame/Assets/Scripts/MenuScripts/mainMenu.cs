using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{

    public void startGame() {
        SceneManager.LoadScene("Main");
    }

    public void quitGame() {
        Application.Quit();
    }

    public void goOptions() {
        SceneManager.LoadScene("OptionsMenu");
    }   

    public void goMenu() {
        SceneManager.LoadScene("MainMenu");
    } 


}
