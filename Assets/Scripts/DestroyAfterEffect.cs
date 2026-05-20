using UnityEngine;
using System.Collections;

public class DestroyAfterEffect : MonoBehaviour
{
    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(DeactivateRoutine());
    }

    private IEnumerator DeactivateRoutine()
    {
        Animator animator = GetComponent<Animator>();
        if (animator == null)
        {
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
            yield break;
        }

        // Wait one frame for the animator to initialize
        yield return null;

        float length = animator.GetCurrentAnimatorStateInfo(0).length;
        if (length <= 0) length = 1f;

        yield return new WaitForSeconds(length);
        gameObject.SetActive(false);
    }
}
