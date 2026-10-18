using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;


public class VideoMenuManager : MonoBehaviour
{
    
    [SerializeField] private TMP_Dropdown dropdown;

    enum AspectRatio
    {
       SIXTEEN_BY_NINE ,
       SIXTEEN_BY_TEN,
       TWENTY_ONE_BY_TEN
        
    }

    private const float TOLERANCE = 0.1f;
    
    [SerializeField] private AspectRatio aspectRatio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        dropdown.ClearOptions();
        List<Resolution> resolutions = new List<Resolution>();
        
        foreach (Resolution resolution in Screen.resolutions)
        {
            float resolutionAspectRatio = (float) resolution.width / resolution.height;
            Debug.Log(resolutionAspectRatio + " Res: " + resolution + " Result: " + (Math.Abs(resolutionAspectRatio - 1.7) < 0.1) );
            
            if(aspectRatio == AspectRatio.SIXTEEN_BY_NINE && Math.Abs(resolutionAspectRatio - 1.7) < TOLERANCE)
                resolutions.Add(resolution);
            else if(aspectRatio == AspectRatio.SIXTEEN_BY_TEN && Math.Abs(resolutionAspectRatio - 1.6000) < TOLERANCE)
                resolutions.Add(resolution);
            else if(aspectRatio == AspectRatio.TWENTY_ONE_BY_TEN && Math.Abs(resolutionAspectRatio - 2.3333) < TOLERANCE)
                resolutions.Add(resolution);
         
           
           
    
           
        }

        resolutions = new List<Resolution>(Screen.resolutions
            .OrderByDescending(r => r.width)
            .ThenByDescending(r => r.height)
            .ThenByDescending(r => r.refreshRateRatio.value)); // Use r.refreshRate on older Unity versions
           
        List<string> options = resolutions
            .Select(r => $"{r.width} x {r.height} @ {r.refreshRateRatio.value:0}Hz")
            .ToList();
        dropdown.AddOptions(options);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
