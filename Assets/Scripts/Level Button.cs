using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int level;
    
    void Start()
    {
        Button button = GetComponent<Button>();

        if (PlayerPrefs.GetInt("Level Reached") < level) 
        {
            button.interactable = false;
        }
    }

    
}
