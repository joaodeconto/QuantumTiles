using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    private GameInputActions gameInputActions;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;

    private void Awake()
    {
        gameInputActions = new GameInputActions();

        // Subscribe to the SelectCard action with Modifier for click and position
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
        // Check if card flipping is allowed in the GameManager
        if (!GameManager.Instance.CanSelectCard()) return;

        // Check if the pointer is over any UI element using GraphicRaycaster
        if (IsPointerOverUI(context.ReadValue<Vector2>())) return;

        // Get the screen position from the SelectCard action's Modifier Composite
        Vector2 screenPosition = context.ReadValue<Vector2>();

        // Perform a raycast from the screen position
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

    private bool IsPointerOverUI(Vector2 position)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = position
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, raycastResults);

        // Loop through raycast results and return true only if we hit a UI element other than the Canvas
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject != graphicRaycaster.gameObject) // Ignore the Canvas itself
            {
                return true;
            }
        }

        return false;
    }
}
