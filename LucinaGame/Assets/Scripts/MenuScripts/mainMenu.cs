using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{

    public void startGame() {
        SceneManager.LoadScene("MAIN_MAP");
    }

    public void quitGame() {
        Application.Quit();
    }

    public void goOptions() {
        SceneManager.LoadScene("OPTIONS_MENU");
    }   

    public void goMenu() {
        SceneManager.LoadScene("MAIN_MENU");
    } 


}
