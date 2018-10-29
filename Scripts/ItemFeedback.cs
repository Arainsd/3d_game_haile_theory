using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemFeedback : MonoBehaviour
{
    [SerializeField]
    private RectTransform feedback;

    [SerializeField]
    private Player player;

    private void Start()
    {
        setVisibility(false);
    }

    private bool textEnabled = false;

    private void Update()
    {
        bool hasItem = player.itemInHand != null;

        if (hasItem && !textEnabled)
            setVisibility(true);
        else if (!hasItem && textEnabled)
            setVisibility(false);
    }

    private void setVisibility(bool visible)
    {
        textEnabled = visible;

        foreach (CanvasRenderer renderer in this.FindComponents<CanvasRenderer>(RedUtil.FindMode.CHILDREN_AND_SELF))
        {
            renderer.SetAlpha(visible ? 1f : 0f);
        }
    }
}
