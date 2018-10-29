using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IEnumerable
{
    private void Awake()
    {
        this.content = new List<InventoryStack>();

        foreach (InventoryStack stack in GetComponents<InventoryStack>())
        {
            this.content.Add(stack);
        }
    }

    [SerializeField]
    private int size;

    public int maximumSize
    {
        get
        {
            return size;
        }
    }

    private List<InventoryStack> _content = new List<InventoryStack>();

    /// <summary>
    /// The list that makes up the actual inventory.
    /// </summary>
    [SerializeField]
    [HideInInspector]
    private List<InventoryStack> content
    {
        get
        {
            return _content;
        }

        set
        {
            _content = value;
        }
    }

    public InventoryStack this[int i] // Overload array getter to act as List
    {
        get
        {
            if (i < 0 || i >= content.Count)
                return null;

            return content[i];
        }
    }

    public int Count
    {
        get
        {
            if (content == null)
                return 0;

            return content.Count;
        }
    }

    // IEnumerator and IEnumerable support

    public IEnumerator<InventoryStack> GetEnumerator()
    {
        return content.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool isEmpty
    {
        get
        {
            return content.Count == 0;
        }
    }

    public bool isFull
    {
        get
        {
            return content.Count >= size;
        }
    }

    /// <summary>
    /// Get the index of the object.
    /// <para>The object should be one of the following to get a correct value back:</para>
    /// <para>The stack's pickup component.</para>
    /// <para>The stack's text component in the inventory.</para>
    /// <para>The game object of the stack's pickup component.</para>
    /// <para>The stack itself. <see cref="InventoryStack"/></para>
    /// </summary>
    /// <returns></returns>
    public int IndexOf(object obj)
    {
        return content.FindIndex((p) => p.IsSame(obj));
    }

    /// <summary>
    /// Does this inventory contain our object?
    /// <para>The object should be one of the following to get a correct value back:</para>
    /// <para>The stack's pickup component.</para>
    /// <para>The stack's text component in the inventory.</para>
    /// <para>The game object of the stack's pickup component.</para>
    /// <para>The stack itself. <see cref="InventoryStack"/></para>
    /// </summary>
    /// <returns></returns>
    public bool Contains(object obj)
    {
        foreach (InventoryStack stack in content)
        {
            if (stack.IsSame(obj))
                return true;
        }

        return false;
    }

    public void Clear()
    {
        List<InventoryStack> clone = new List<InventoryStack>(content);

        content.Clear();

        for (int i = 0; i < clone.Count; i++)
        {
            InventoryStack stack = clone[i];

            if (stack != null && OnRemove != null)
                OnRemove(stack, i);

            Destroy(stack.image.gameObject);
            Destroy(stack);
        }
    }

    /// <summary>
    /// Attempt to get our InventoryStack by it's object.
    /// <para>The object should be one of the following to get a correct value back:</para>
    /// <para>The stack's pickup component.</para>
    /// <para>The stack's text component in the inventory.</para>
    /// <para>The game object of the stack's pickup component.</para>
    /// <para>The stack itself. <see cref="InventoryStack"/></para>
    /// </summary>
    /// <returns></returns>
    public InventoryStack Get(object obj)
    {
        foreach (InventoryStack stack in content)
        {
            if (stack.IsSame(obj))
                return stack;
        }

        return null;
    }

    /// <summary>
    /// Event handler called whenever an item gets removed from the inventory.
    /// The int is the index of the inventory stack being removed.
    /// </summary>
    public event Action<InventoryStack, int> OnRemove;

    /// <summary>
    /// Attempt to remove something from the inventory.
    /// <para>The object should be one of the following to remove a correct inventory stack:</para>
    /// <para>The stack's pickup component.</para>
    /// <para>The stack's text component in the inventory.</para>
    /// <para>The game object of the stack's pickup component.</para>
    /// <para>The stack itself. <see cref="InventoryStack"/></para>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>If the object was succesfully removed or not.</returns>
    public bool Remove(object obj)
    {
        for (int i = 0; i < content.Count; i++)
        {
            InventoryStack stack = content[i];

            if (stack.IsSame(obj))
            {
                content.Remove(stack);

                if (OnRemove != null)
                    OnRemove(stack, i);

                Destroy(stack.image.gameObject);
                Destroy(stack);

                return true;
            }
        }

        return false;
    }

    public event Action<InventoryStack> OnAdd;
         
    /// <summary>
    /// Add a pickup to the inventory.
    /// </summary>
    /// <param name="item">"The pickup component of the item</param>
    /// <param name="text">The text component in the UI</param>
    public InventoryStack Add(Pickup item, GameObject parent)
    {
        InventoryStack stack = gameObject.AddComponent<InventoryStack>();
        stack.item = item;
        stack.image = createIcon(item, parent);
        content.Add(stack);

        if (OnAdd != null)
            OnAdd(stack);
    
        return stack;
    }

    /// <summary>
    /// Creates an icon of the pickup, to be used in the inventory.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private Image createIcon(Pickup item, GameObject parent)
    {
        //get the name of the item without the text in parenthesis
        string itemName = item.gameObject.ToString().Split('(')[0];

        //create new gameObject for the text and add it to the notepad
        GameObject container = new GameObject(itemName);
        container.transform.SetParent(parent.transform);

        //add text component to the new gameObject
        Image image = container.AddComponent<Image>();
        image.sprite = item.inventoryIcon;

        GameObject textContainer = new GameObject(itemName);
        textContainer.transform.SetParent(container.transform);

        //Text text = textContainer.AddComponent<Text>();
        //text.text = itemName;
        //text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        //text.alignment = TextAnchor.MiddleCenter;
        //text.rectTransform.rotation = Quaternion.Euler(0, 0, 20);
        //text.fontSize = 10;

        return image;
    }

    /// <summary>
    /// Executes a function for each of the stacks in the inventory.
    /// </summary>
    /// <param name="p"></param>
    public void ForEach(Action<InventoryStack> action)
    {
        content.ForEach(action);
    }
}

/// <summary>
/// A single item in an inventory.
/// </summary>
public class InventoryStack : MonoBehaviour
{
    /// <summary>
    /// The Pickup component of the item's game object.
    /// </summary>
    public Pickup item;

    /// <summary>
    /// The image component that is used to display the inventory.
    /// </summary>
    public Image image;

    /// <summary>
    /// Is this stack the same as the object?
    /// Checks if any component of this stack is equal to the object.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool IsSame(object obj)
    {
        if (obj is Pickup)
            return obj.Equals(item);

        if (obj is GameObject && item != null)
            return obj.Equals(item.gameObject);

        if (obj is Image)
            return obj.Equals(image);

        return base.Equals(obj);
    }
}

