using Sandbox.DuarteFonseca;
using UnityEngine;
using UnityEngine.InputSystem;

public readonly struct PlayerActions
{
    // Read-only metadata source used strictly for compile-time GUIDs, to distinguish between owners
    private static PlayerInputActions _metadata;
    private static PlayerInputActions Metadata => _metadata ??= new PlayerInputActions();

   //Actions on the Generated PlayerInputActions C# class, from the PlayerActions InputMap
   //not sure if this should be private, but since they are readonly it's mostly fine
    public readonly InputAction Move;
    public readonly InputAction Look;
    public readonly InputAction Jump;
    public readonly InputAction Sprint;
    public readonly InputAction Interact;

    public PlayerActions(PlayerInput playerInput)
    {
        var asset = playerInput.actions;
        var map = Metadata.Player;

        Move     = asset.FindAction(map.Move.id);
        Look     = asset.FindAction(map.Look.id);
        Jump     = asset.FindAction(map.Jump.id);
        Sprint   = asset.FindAction(map.Sprint.id);
        Interact = asset.FindAction(map.Interact.id);
    }
    
    
    //Helpers
    public Vector2 MoveVector => Move.ReadValue<Vector2>();
    public Vector2 LookVector => Look.ReadValue<Vector2>();
    
    public float JumpVector => Jump.ReadValue<float>();
}