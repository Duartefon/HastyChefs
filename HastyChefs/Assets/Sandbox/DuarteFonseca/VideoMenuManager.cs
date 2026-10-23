using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class VideoMenuManager : MonoBehaviour
{
    
    [SerializeField] private TMP_Dropdown dropdownResolution, dropdownFullscreenType;

    enum AspectRatio
    {
       SIXTEEN_BY_NINE ,
       SIXTEEN_BY_TEN,
       TWENTY_ONE_BY_TEN
        
    }

    private const float TOLERANCE = 0.09f;
    List<Resolution> resolutions = new();
    [SerializeField] private AspectRatio aspectRatio;
    



    [SerializeField] private Toggle toggleSixteenByNine, toggleSixteenByTen, toggleTwentyOneByNine;
    
    private FullScreenMode fullScreenMode;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        SetupListeners();

        InitializeResolutionList();

        InitializeFullScreenType();


    }

    private void SetupListeners()
    {
        dropdownResolution.onValueChanged.AddListener(OnResolutionChanged);
        
        dropdownFullscreenType.onValueChanged.AddListener(OnFullscreenModeChanged);
        
        toggleSixteenByNine.onValueChanged.AddListener((isOn) => 
        {
            if (isOn) OnAspectRatioChanged(AspectRatio.SIXTEEN_BY_NINE);
        });

        toggleSixteenByTen.onValueChanged.AddListener((isOn) => 
        {
            if (isOn) OnAspectRatioChanged(AspectRatio.SIXTEEN_BY_TEN);
        });

        toggleTwentyOneByNine.onValueChanged.AddListener((isOn) => 
        {
            if (isOn) OnAspectRatioChanged(AspectRatio.TWENTY_ONE_BY_TEN);
        });

    }

    private void InitializeResolutionList()
    {
        dropdownResolution.options.Clear();
        resolutions.Clear();
        
        foreach (Resolution resolution in Screen.resolutions)
        {
            float resolutionAspectRatio = (float) resolution.width / resolution.height;
            Debug.Log(resolutionAspectRatio + " Res: " + resolution + " AspectRatio " + resolutionAspectRatio  + " Result: " + (Math.Abs(resolutionAspectRatio - 1.777) <=  TOLERANCE ) + " Refresh Rate: " + resolution.refreshRateRatio );
            
            if(aspectRatio == AspectRatio.SIXTEEN_BY_NINE && Math.Abs(resolutionAspectRatio - 1.777) <= TOLERANCE)
                resolutions.Add(resolution);
            else if(aspectRatio == AspectRatio.SIXTEEN_BY_TEN && Math.Abs(resolutionAspectRatio - 1.6000) <= TOLERANCE)
                resolutions.Add(resolution);
            else if(aspectRatio == AspectRatio.TWENTY_ONE_BY_TEN && Math.Abs(resolutionAspectRatio - 2.3333) <= TOLERANCE)
                resolutions.Add(resolution);
        }
        
        resolutions = resolutions.OrderByDescending(r => r.width)
            .ThenByDescending(r => r.height)
            .ThenByDescending(r => r.refreshRateRatio.value).ToList();
        
        List<string> optionsList = resolutions
            .Select(r => $"{r.width} x {r.height} @ {r.refreshRateRatio.value:0}Hz")
            .ToList();
        
        dropdownResolution.AddOptions(optionsList);
    }


    private void InitializeFullScreenType()
    {
        List<String> fullscrenOptions = new();
        dropdownFullscreenType.options.Clear();
        
        foreach (FullScreenMode fullscreenType in Enum.GetValues(typeof(FullScreenMode)))
        {
            Debug.Log("FullscreenType:" + fullscreenType);
            fullscrenOptions.Add(fullscreenType.ToString());
            
        }
        
        dropdownFullscreenType.AddOptions(fullscrenOptions);
        
    }

    private void OnFullscreenModeChanged(int value)
    {
        fullScreenMode = (FullScreenMode) value; 
        
        #if UNITY_EDITOR
            Debug.Log("FullscreenMode:" + fullScreenMode);
        #endif
        
        
        Screen.fullScreenMode = fullScreenMode;
        
        
    }
    private void OnResolutionChanged(int value)
    {
        Debug.Log( "Changed Res to: " + resolutions[value].width + " Res: " + resolutions[value].height );
        var resolution = resolutions[value];
        Screen.SetResolution(resolution.width, resolution.height, fullScreenMode, resolution.refreshRate);
      
    }

    private void OnAspectRatioChanged(AspectRatio value)
    {
      aspectRatio =  value;
      InitializeResolutionList();      
    }
}
