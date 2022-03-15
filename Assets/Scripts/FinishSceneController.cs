using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishSceneController : MonoBehaviour {


    void Start()
    {
        Cursor.visible = true;
         Screen.lockCursor = false;
         Time.timeScale = 1; 
    }

    public void PassToDockScene()
    {
        Debug.Log("dock--");
        SceneManager.LoadScene("Dock Thing");
    }

    public void PassToIndustryScene()
    {
        Debug.Log("industry--");
        SceneManager.LoadScene("industry");  
    }

}
