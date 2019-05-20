using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainAnim : MonoBehaviour
{

    private GameObject _curtain;
    private GameObject _openCurtain;

    private void Start()
    {
        _curtain = GameObject.Find("Curtain1");
        _openCurtain = GameObject.Find("CurtainAnim");
    }

    private void OnMouseDown()
    {
        _curtain.SetActive(false);
        _openCurtain.SetActive(true);
    }

}
