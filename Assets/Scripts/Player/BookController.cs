using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BookController : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer bookRenderer;
    Animator bookAnim;
    CharAnimations charAnimations;
    CompositeStateToken freezePlayer;
    Book hoveredBook;
    public Book pickedUpBook { get;private set; }
    public bool HasBook => pickedUpBook != null;

    public UnityEvent onBookEvent = new UnityEvent();

    InputMap inputs;

    // Start is called before the first frame update
    void Start()
    {
        bookAnim = bookRenderer.GetComponent<Animator>();
        charAnimations = PlayerState.Instance.transform.GetComponentInChildren<CharAnimations>();
        freezePlayer = new CompositeStateToken();
        PlayerState.Instance.freezePositionState.Add(freezePlayer);
        inputs = new InputMap();
        inputs.Gameplay.Enable();
        inputs.Gameplay.Interact.performed += OnInteractInput;
    }

    private void OnInteractInput(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (HasBook)
        {
            DropBook();
            return;
        }

        if (hoveredBook == null)
            return;

        PickUpBook(hoveredBook);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Book"))
            return;

        if (!other.TryGetComponent(out Book book))
            return;

        if (hoveredBook != null)
            hoveredBook.Hover(false);
        hoveredBook = book;
        hoveredBook.Hover(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Book"))
            return;

        if (!other.TryGetComponent(out Book book))
            return;

        if (book == hoveredBook)
        {
            hoveredBook.Hover(false);
            hoveredBook = null;
        }
    }

    public void PickUpBook(Book book)
    {
        if (HasBook)
            pickedUpBook.HideBook(false);

        pickedUpBook = book;
        pickedUpBook.HideBook(true);
        book.SetTexture();
        bookRenderer.sharedMaterials = book.SharedMaterials;
        PlayerState.Instance.GetComponentInChildren<CharController>().LerpToPosition(book.transform.position);
        charAnimations.PickUpBook(true);
        bookAnim.SetBool("IsPickedUp", true);
        freezePlayer.SetOn(true);
        SFXManager.PlaySound(GlobalSFX.OpenBook);
        onBookEvent.Invoke();
    }

    public void DropBook()
    {
        pickedUpBook.HideBook(false);
        pickedUpBook = null;
        charAnimations.PickUpBook(false);
        bookAnim.SetBool("IsPickedUp", false);
        freezePlayer.SetOn(false);
        SFXManager.PlaySound(GlobalSFX.CloseBook);
        onBookEvent.Invoke();
    }
}
