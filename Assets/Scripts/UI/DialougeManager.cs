using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public Animator dialougeAnimator;

    private float autoHideTime = 3f;
    private float typingSpeed = 0.1f;

    private Coroutine hideCoroutine;
    private Coroutine typingCoroutine;

    public void ShowDialogue(string message)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        dialogueBox.SetActive(true);
        dialogueText.text = "";

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        yield return new WaitForSeconds(.6f); 

        foreach (char c in message)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(autoHideTime);
        dialougeAnimator.SetTrigger("popout");
        yield return new WaitForSeconds(0.3f);
        dialogueBox.SetActive(false);
    }
}
