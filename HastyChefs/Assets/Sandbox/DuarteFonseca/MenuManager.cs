using UnityEngine;
using UnityEngine.Rendering;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private GameObject mainMenuOption, singleplayerMenu, multiplayerMenu, optionMenu;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

