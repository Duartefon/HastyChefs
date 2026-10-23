using Tweens;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Menu
{
    public class MenuManager : MonoBehaviour
    {

        [SerializeField] private GameObject mainMenuOption, singleplayerMenu, multiplayerMenu, optionMenu;

        [SerializeField] private Button  singleplayerMenuButton, multiplayerMenuButton, optionMenuButton, quitButton;
        // Start is called once before the first execution of Update after the MonoBehaviour is created


        void Start()
        {
      
            Screen.SetResolution(1920,1080, true);
    
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
            
                optionMenu.GetComponent<IAnimatedView>().Show();
                mainMenuOption.GetComponent<IAnimatedView>().Hide();
            
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
}

