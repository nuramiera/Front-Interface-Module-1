using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ModuleMenu : MonoBehaviour
{
    public Button Module1;
    public Button Module2;
    public Button Module3;
    public Button Module4;

    public void GotoModule1()
    {
        SceneManager.LoadScene("Pick up cube & controller show");

    }

    public void GotoModule2()
    {
        SceneManager.LoadScene("Game");

    }

    public void GotoModule3()
    {
        SceneManager.LoadScene("final");

    }

    public void GotoModule4()
    {
        SceneManager.LoadScene("Game");

    }
}
