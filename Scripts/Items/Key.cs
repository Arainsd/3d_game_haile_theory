using UnityEngine;
using System.Collections;

public class Key : Pickup
{
    [SerializeField]
    private Door[] doors;

    public bool singleUse = true;

    /// <summary>
    /// Can this key open this door?
    /// </summary>
    /// <param name="door"></param>
    /// <returns></returns>
    public virtual bool Match(Door door)
    {
        foreach (Door other in doors)
        {
            if (other.Equals(door))
                return true;
        }

        return false;
    }
}
