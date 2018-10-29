using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Shrinkable : MonoBehaviour
{
    [HideInInspector]
    public Vector3 originalScale;

    [SerializeField]
    [Tooltip("Which renderer should display the outline for the shrinkable feedback? (If empty, uses itself)")]
    public Renderer outlineRenderer;

    public Vector3 minimumSize = new Vector3(0.1f, 0.1f, 0.1f);

    private void Start()
    {
        originalScale = transform.localScale;
    }
}
