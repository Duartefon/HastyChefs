using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TabManager : MonoBehaviour
{
    [SerializeField] public GameObject[] tabs;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject mainMenu;

    public void Start()
    {
        backButton.onClick.AddListener( () => { gameObject.SetActive(false); mainMenu.SetActive(true); });
    }
    

    public void OnTabSwitch(GameObject tab)
    {
        tab.SetActive(true);

        for (int i = 0; i < tabs.Length; i++)
        {
            if(tabs[i] != tab)
            {
                tabs[i].SetActive(false);
            }
        }
    }
    
    

}
