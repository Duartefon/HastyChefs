using System;
using PrimeTween;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject mainMenuOption, singleplayerMenu, multiplayerMenu, optionMenu;

    [SerializeField] private Button  singleplayerMenuButton, multiplayerMenuButton, optionMenuButton, quitButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
 

    void Start()
    {
      
   
         
#if UNITY_EDITOR
        if (!mainMenuOption || !multiplayerMenu || !singleplayerMenu || !optionMenu )
           Debug.LogWarning($"One of the references is missing! " +
                            $"mainMenu : {mainMenuOption}, " +
                            $"singleplayerMenu: {singleplayerMenu}, " +
                            $"multiplayerMenu: {multiplayerMenu} , " +
                            $"optionMenu: {optionMenu}   " +
                            $"has not been correctly referenced!");
#endif

        singleplayerMenuButton.onClick.AddListener(() =>
        {
            singleplayerMenu.SetActive(true); mainMenuOption.SetActive(false); 
        });
        
        multiplayerMenuButton.onClick.AddListener(() =>
        {
            multiplayerMenu.SetActive(true); mainMenuOption.SetActive(false); 
        });

        optionMenuButton.onClick.AddListener(() =>
        {
            optionMenu.SetActive(true); mainMenuOption.SetActive(false);  
            Tween.Scale(optionMenu.transform, startValue: 0, endValue: 2f, duration: 0.5f, ease: Ease.InOutElastic, startDelay: 1f);
        });
        
        quitButton.onClick.AddListener(Application.Quit);
        
        //multiplayerMenuButton.onClick.AddListener(OnClicked);
        
        
    }

 

    void OnClicked()
    {
        mainMenuOption.SetActive(false);
        singleplayerMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

