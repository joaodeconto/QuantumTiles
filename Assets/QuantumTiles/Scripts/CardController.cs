using TMPro;
using UnityEngine;

public class CardController : MonoBehaviour
{    
    [SerializeField] private Animator animator;
    [SerializeField] private bool isFlipped = false;
    [SerializeField] private bool isMatched = false;
    [SerializeField] private TMP_Text _cardIdText;


    // Unique identifier or matching value for each card
    private int _cardID;

    public delegate void CardFlipped(CardController card);
    public static event CardFlipped OnCardFlipped;

    public int CardID 
    { 
        get 
        { 
            return _cardID; 
        } 

        set 
        {  
            _cardID = value; 
            SetCardVisual(); 
        } 
    }

    private void Awake()
    {
        if(animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void SetCardVisual()
    {
        _cardIdText.text = _cardID.ToString();
    }


    public void FlipCard()
    {
        // Set the flipped state to true and play the flip animation
        isFlipped = true;
        animator.SetTrigger("Flip");
        //animator.SetBool("IsFlipped", isFlipped);

        // Notify GameManager that this card was flipped
        OnCardFlipped?.Invoke(this);
    }

    public void MatchCard()
    {
        // Set matched state and play match animation
        isMatched = true;
        animator.SetTrigger("Match");

        //TODO set join animation
        Destroy(this.gameObject, 1);
    }

    public void UnflipCard()
    {
        // Reset flipped state and play flip-back animation
        animator.SetTrigger("Mismatch");
        isFlipped = false;
        //animator.SetBool("IsFlipped", isFlipped);
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
