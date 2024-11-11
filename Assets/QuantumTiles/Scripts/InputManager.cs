using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private GameInputActions gameInputActions;

    private void Awake()
    {
        gameInputActions = new GameInputActions();
        
        // Subscribe to the SelectCard action
        gameInputActions.Player.SelectCard.performed += OnSelectCard;
    }

    private void OnEnable()
    {
        gameInputActions.Enable();
    }

    private void OnDisable()
    {
        gameInputActions.Disable();
    }

    private void OnSelectCard(InputAction.CallbackContext context)
    {
        // Raycast from the mouse position (or touch position)
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the object has a CardController
            CardController card = hit.collider.GetComponent<CardController>();
            if (card != null && !card.IsFlipped() && !card.IsMatched())
            {
                card.FlipCard();
            }
        }
    }
}
