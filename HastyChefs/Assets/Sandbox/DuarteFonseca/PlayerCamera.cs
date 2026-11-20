
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]

public class PlayerCam : MonoBehaviour
{

    [SerializeField] private  float sensX;
    [SerializeField] private  float sensY;
    [SerializeField] private  float maxY = 90f;
    [SerializeField] private  float maxX = 90f;
    [SerializeField] private float modelTurnSpeed = 15f;
    
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform playerModel;
    
    private float _horizontalRotation;
    private float _verticalRotation;

    private float _defaultSens;

    private PlayerActions _playerInput;


    public void setSens(int sensValue)
    {
        sensX = sensValue;
        sensY = sensValue; 
    }

    public void resetSens()
    {
        sensX = _defaultSens;
        sensY = _defaultSens;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _defaultSens = sensX;
        
        
        PlayerInput playerInput = GetComponent<PlayerInput>();
        _playerInput =  new PlayerActions(playerInput);

       
    }

    // Update is called once per frame
    void LateUpdate()  
    {   
        Vector2 deltaMouse = _playerInput.LookVector;
        
        float mouseX = deltaMouse.x  * sensX;
        float mouseY = deltaMouse.y  * sensY;

        _verticalRotation += mouseY;
        _horizontalRotation += mouseX;
        
        _verticalRotation = Mathf.Clamp( _verticalRotation, -maxY , maxY);

        
        cameraHolder.localRotation = Quaternion.Euler(-_verticalRotation, _horizontalRotation, 0);
  
        
       
       
       // 2. Smoothly align the human 3D model's Y-rotation to match look direction
         Quaternion targetBodyRotation = Quaternion.Euler(0f, _horizontalRotation, 0f);
         playerModel.localRotation = Quaternion.Slerp(
             playerModel.localRotation, 
             targetBodyRotation,
             modelTurnSpeed * Time.deltaTime
         );
    }
}
