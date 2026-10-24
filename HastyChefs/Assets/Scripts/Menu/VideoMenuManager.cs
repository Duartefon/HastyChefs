using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    
    //TODO: Change how this works so that resolution is changed through URP Scaling, so that UI does not get blurry
    
    /// <summary>
    /// Manages the options in the video tab of the Options menu.
    /// Currently, can change: Resolution, AspectRatio and FullScreen Mode
    /// </summary>
    public class VideoMenuManager : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdownResolution, dropdownFullscreenType;

        enum AspectRatio
        {
            SixteenByNine ,
            SixteenByTen,
            TwentyOneByTen
        }
        
        [SerializeField] private AspectRatio aspectRatio;
        [SerializeField] private Toggle toggleSixteenByNine, toggleSixteenByTen, toggleTwentyOneByNine;
        
        private const float Tolerance = 0.09f;
        
        private List<Resolution> _resolutions = new();
        private FullScreenMode _fullScreenMode;
        
        
 
        void Start()
        {

            SetupListeners();

            InitializeResolutionList();

            InitializeFullScreenType();


        }
        
        /// <summary>
        /// For each button and dropdown, setups the corresponding onClick or onValueChanged event, to a function or
        /// lambda function.
        /// </summary>

        private void SetupListeners()
        {
            dropdownResolution.onValueChanged.AddListener(OnResolutionChanged);
        
            dropdownFullscreenType.onValueChanged.AddListener(OnFullscreenModeChanged);
        
            toggleSixteenByNine.onValueChanged.AddListener((isOn) => 
            {
                if (isOn) OnAspectRatioChanged(AspectRatio.SixteenByNine);
            });

            toggleSixteenByTen.onValueChanged.AddListener((isOn) => 
            {
                if (isOn) OnAspectRatioChanged(AspectRatio.SixteenByTen);
            });

            toggleTwentyOneByNine.onValueChanged.AddListener((isOn) => 
            {
                if (isOn) OnAspectRatioChanged(AspectRatio.TwentyOneByTen);
            });

        }
     /// <summary>
     /// This method reads the available screen resolutions and populates the resolution list with the
     /// resolutions available in that aspect ratio. Since unity does not provide the aspect ratio for a resolution,
     /// it's manually checked for each resolution by dividing width by height and then checking if the result is the as
     /// the selected aspect ratio
     /// </summary>
        private void InitializeResolutionList()
        {
            dropdownResolution.options.Clear();
            _resolutions.Clear();
        
            foreach (Resolution resolution in Screen.resolutions)
            {
                
                float resolutionAspectRatio = (float) resolution.width / resolution.height;
                Debug.Log(resolutionAspectRatio + " Res: " + resolution + " AspectRatio " + resolutionAspectRatio  + " Result: " + (Math.Abs(resolutionAspectRatio - 1.777) <=  Tolerance ) + " Refresh Rate: " + resolution.refreshRateRatio );
            
                if(aspectRatio == AspectRatio.SixteenByNine && Math.Abs(resolutionAspectRatio - 1.777) <= Tolerance)
                    _resolutions.Add(resolution);
                else if(aspectRatio == AspectRatio.SixteenByTen && Math.Abs(resolutionAspectRatio - 1.6000) <= Tolerance)
                    _resolutions.Add(resolution);
                else if(aspectRatio == AspectRatio.TwentyOneByTen && Math.Abs(resolutionAspectRatio - 2.3333) <= Tolerance)
                    _resolutions.Add(resolution);
            }
        
            _resolutions = _resolutions.OrderByDescending(r => r.width)
                .ThenByDescending(r => r.height)
                .ThenByDescending(r => r.refreshRateRatio.value).ToList();
        
            List<string> optionsList = _resolutions
                .Select(r => $"{r.width} x {r.height} @ {r.refreshRateRatio.value:0}Hz")
                .ToList();
        
            dropdownResolution.AddOptions(optionsList);
        }

        /// <summary>
        /// This method reads the different options of fullscreen modes and the fullscreen list with each option.
        /// </summary>
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
        /// <summary>
        /// This method applies the type of fullscreen,  when a type from the fullscreen list is selected
        /// <param name="value"> An integer that represents the option selected, being converted to FullScreenMode</param>
        /// </summary>
        private void OnFullscreenModeChanged(int value)
        {
            _fullScreenMode = (FullScreenMode) value; 
            
            Screen.fullScreenMode = _fullScreenMode;
        }
        
        /// <summary>
        /// This method changes the resolution, when a new resolution from the resolution list.
        /// <param name="value"> An integer that represents the index of the resolution selected</param>
        /// </summary>
        private void OnResolutionChanged(int value)
        {
            Debug.Log( "Changed Res to: " + _resolutions[value].width + " Res: " + _resolutions[value].height );
            var resolution = _resolutions[value];
            Screen.SetResolution(resolution.width, resolution.height, _fullScreenMode, resolution.refreshRate);
      
        }

        private void OnAspectRatioChanged(AspectRatio value)
        {
            aspectRatio =  value;
            InitializeResolutionList();      
        }
    }
}
