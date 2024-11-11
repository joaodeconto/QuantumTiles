using UnityEngine;

public class CardController : MonoBehaviour
{    
    [SerializeField] private Animator animator;

    private bool isFlipped = false;
    private bool isMatched = false;

    public delegate void CardFlipped(CardController card);
    public static event CardFlipped OnCardFlipped;

    // Unique identifier or matching value for each card
    public int cardID;

    private void Awake()
    {
        if(animator == null)
            animator = GetComponentInChildren<Animator>();
    }


    public void FlipCard()
    {
        // Set the flipped state to true and play the flip animation
        isFlipped = true;
        animator.SetTrigger("Flip");

        // Notify GameManager that this card was flipped
        OnCardFlipped?.Invoke(this);
    }

    public void MatchCard()
    {
        // Set matched state and play match animation
        isMatched = true;
        animator.SetTrigger("Match");
    }

    public void UnflipCard()
    {
        // Reset flipped state and play flip-back animation
        isFlipped = false;
        animator.SetTrigger("FlipBack");
    }

    public bool IsFlipped()
    {
        return isFlipped;
    }

    public bool IsMatched()
    {
        return isMatched;
    }
}
