using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>
/// Draws an inventory to a RectTransform
/// </summary>
public class InventoryDrawer : MonoBehaviour
{
    [SerializeField]
    public Inventory inventory;

    [SerializeField]
    public RectTransform drawPanel;

    public void Awake()
    {
        Refresh();
    }

    private void OnValidate()
    {
        Refresh();
    }

    /// <summary>
    /// Refresh the rendering of the inventory.
    /// </summary>
    public void Refresh()
    {
        if (drawPanel == null || inventory == null)
            return;

        updateContainer();
        updateIcons();

        inventory.OnAdd -= OnItemAdd; // Unregister
        inventory.OnRemove -= OnItemRemove; // Unregister

        inventory.OnAdd += OnItemAdd;
        inventory.OnRemove += OnItemRemove;
    }

    private void OnItemRemove(InventoryStack stack, int index)
    {
        Refresh();
    }

    private void OnItemAdd(InventoryStack stack)
    {
        Refresh();
    }

    [SerializeField]
    private int iconsPerRow = 5;

    [SerializeField]
    private int iconWidth = 64;

    [SerializeField]
    private int iconHeight = 64;

    [SerializeField]
    [Tooltip("Describes the size of the border of the inventory.")]
    private int inventoryMargin = 5;

    [SerializeField]
    [Tooltip("Describes the horizontal offset from the last icon and the next icon.")]
    private int iconMarginHorizontal = 5;

    [SerializeField]
    [Tooltip("Describes the vertical offset from the last icon and the next icon.")]
    private int iconMarginVertical = 5;

    /// <summary>
    /// The offset to the local position of the inventory container that moves a child to the left-bottom corner of it.
    /// Calculates when updating the container size.
    /// </summary>
    private Vector3 positionOffset;

    /// <summary>
    /// Total amount of items we have to draw.
    /// </summary>
    private int drawCount
    {
        get
        {
            if (inventory == null)
                return 0;

            return inventory.maximumSize;
        }
    }

    /// <summary>
    /// How much rows we have to draw.
    /// </summary>
    private int rows;

    /// <summary>
    /// How much columns we have to draw.
    /// </summary>
    private int cols;

    /// <summary>
    /// Resize and update the container of the inventory.
    /// </summary>
    private void updateContainer()
    {
        int items = Math.Max(drawCount, 1); // Make room for at least one item.

        cols = iconsPerRow;
        rows = Mathf.FloorToInt(items / (float)iconsPerRow);

        int width = inventoryMargin + (cols * iconWidth) + (Math.Max(0, cols - 1) * iconMarginHorizontal) + inventoryMargin; // Calculate entire width of the container.
        int height = inventoryMargin + (rows * iconHeight) + (Math.Max(0, rows - 1) * iconMarginVertical) + inventoryMargin; // Calculate height of the container.

        // Apply size
        drawPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        drawPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        positionOffset = -new Vector3(width / 2f, height / 2f, 0) + new Vector3(inventoryMargin, inventoryMargin, 0);
    }

    /// <summary>
    /// Reposition icons in the new container.
    /// </summary>
    private void updateIcons()
    {
        float fillX = 0;

        int lastRow = drawCount % iconsPerRow;

        if (lastRow < iconsPerRow && iconsPerRow % 2 == 1 && rows > 1) // If our last row is not completely filled, we need to offset it so that it is centered.
        {
            fillX = (iconsPerRow - lastRow) * (iconWidth / 2f);
        }

        for (int i = 0; i < drawCount; i++)
        {
            if (i >= inventory.Count)
                break; // No need to draw this.

            InventoryStack stack = inventory[i];

            //positions texts one after another on the y axis
            Image image = stack.image;

            image.rectTransform.sizeDelta = new Vector2(iconWidth, iconHeight);

            int column = i % iconsPerRow;
            int row = Mathf.CeilToInt(i / iconsPerRow);

            float xOffset = (iconWidth * column) + (iconMarginHorizontal * column);
            float yOffset = (iconHeight * row) + (iconMarginVertical * row);

            if (row == rows - 1) // Are we the last row?
            {
                xOffset += fillX; // Add the offset we calculated earlier.
            }

            Vector3 position = positionOffset + new Vector3(xOffset + (iconWidth / 2f), yOffset + (iconHeight / 2f), 0);

            image.transform.localPosition = position;
        }
    }
}
