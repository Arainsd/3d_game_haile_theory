using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A Hoverable is an item that can be hovered over by the player, and it will display an outline over the object.
/// It is also the base class for most other items, as they all need to have an outline.
/// </summary>
public abstract class Hoverable : MonoBehaviour
{
    private static Shader outlineShader;

    protected virtual void Awake()
    {
        if (outlineShader == null)
            outlineShader = Shader.Find("Custom/Outline");

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat.shader.Equals(outlineShader))
                {
                    materials.Add(mat);
                }
            }
        }
    }

    private List<Material> materials = new List<Material>();

    public string[] hoverText = new string[4];

    private bool _glow = false;

    /// <summary>
    /// Set/Get if this object currently has it's outline enabled.
    /// </summary>
    public bool glow
    {
        get
        {
            return _glow;
        }
        
        set
        {
            if (value == _glow)
                return;

            _glow = value;

            foreach (Material mat in materials)
            {
                mat.SetFloat("_Enabled", value ? 1 : 0);
            }
        }
    }

    /// <summary>
    /// Called when a player left clicks on this object in the world.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="itemInHand"></param>
    public virtual void LeftClickInWorld(Player player)
    {

    }

    /// <summary>
    /// Called when a player right clicks on this object in the world.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="itemInHand"></param>
    public virtual void RightClickInWorld(Player player)
    {

    }

    /// <summary>
    /// Called when a player starts looking at this object in the world.
    /// </summary>
    /// <param name="player"></param>
    public virtual void StartLookingAtInWorld(Player player)
    {
        glow = true;
    }

    /// <summary>
    /// Called when a player stops looking at this object in the world.
    /// </summary>
    /// <param name="player"></param>
    public virtual void StopLookingAtInWorld(Player player)
    {
        glow = false;
    }
}
