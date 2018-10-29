using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    // The canvas renderers of this screen's children.
    private CanvasRenderer[] canvases;

	private void Awake ()
    {
        canvases = GetComponentsInChildren<CanvasRenderer>();
        active = false; // The screen should start off inactive.
    }
    
    private bool _active;
    public bool active
    {
        get { return _active; }

        set
        {
            // The visibility of this screen's children (0 if value is false).
            float alphaValue = value ? 1f : 0f;

            // Change the visibility of this screen's children to alphaValue.
            foreach (CanvasRenderer renderer in canvases)
            {
                renderer.SetAlpha(alphaValue); 
                renderer.gameObject.SetActive(value);
            }

            _active = value;
        }
    }

}
