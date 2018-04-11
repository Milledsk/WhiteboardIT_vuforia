using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMethods : MonoBehaviour {

    public void Show(GameObject showObject)
    {
        showObject.GetComponent<CanvasGroup>().alpha = 1f;
        showObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void Hide(GameObject hideObject)
    {
        hideObject.GetComponent<CanvasGroup>().alpha = 0f;
        hideObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}
