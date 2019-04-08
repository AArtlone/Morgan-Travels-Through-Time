using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    //Attach an Image you want to fade in the GameObject's Inspector
    public Image m_Image;
    //Use this to tell if the toggle returns true or false
    bool m_Fading;
    // Start is called before the first frame update


    void OnGUI()
    {
        //Fetch the Toggle's state
        m_Fading = GUI.Toggle(new Rect(0, 0, 100, 30), m_Fading, "Fade In/Out");
    }
    // Update is called once per frame
    void Update()
    {

        //If the toggle is false, fade out to nothing (0) the Image with a duration of 2
        if (m_Fading == false)
        {
            m_Image.CrossFadeAlpha(0, 2.0f, false);
        }
    }
    
    }
