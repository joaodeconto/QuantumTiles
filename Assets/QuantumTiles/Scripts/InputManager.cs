using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
        // Use Invoke to delay the call to ProcessCardSelection slightly
        Invoke(nameof(ProcessCardSelection), 0.01f);
    }

    private void ProcessCardSelection()
    {
        // Check if card flipping is allowed in the GameManager
        if (!GameManager.Instance.CanSelectCard()) return;

        // Check if the pointer is currently over any UI element
        if (IsPointerOverUI()) return;

        // Raycast from the mouse position (or touch position)
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CardController card = hit.collider.GetComponent<CardController>();

            // Only flip the card if it is not already flipped or matched
            if (card != null && !card.IsFlipped() && !card.IsMatched())
            {
                card.FlipCard();
            }
        }
    }

    private bool IsPointerOverUI()
    {
        // Returns true if the pointer is over any UI element
        return EventSystem.current.IsPointerOverGameObject();
    }
}
