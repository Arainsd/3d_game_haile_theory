using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// *********************** RED UTIL *********************** //
// Utility classes for Unity
// Made by Stefan Pelskamp
// ******************************************************** //

public static class RedUtil
{
    public enum FindMode
    {
        /// <summary>
        /// Searches the game object in all children.
        /// </summary>
        CHILDREN,
        /// <summary>
        /// Searches the game object in all children and itself.
        /// </summary>
        CHILDREN_AND_SELF,
        /// <summary>
        /// Searches the game object in all parents.
        /// </summary>
        PARENTS,
        /// <summary>
        /// Searches the game object in all parents and itself.
        /// </summary>
        PARENTS_AND_SELF,
        /// <summary>
        /// Searches the game object itself only. (So basically GameObject.GetComponent)
        /// </summary>
        SELF,
        /// <summary>
        /// Searches the game object in all children, parents, and itself.
        /// </summary>
        ALL
        // TODO: Siblings? Closely relative?
    }

    /// <summary>
    /// Attempts to find a component in the game object, for the specific search mode (default is CHILDREN_AND_SELF).
    /// See <see cref="FindMode"/> for descriptions.
    /// </summary>
    /// <param name="mode">What GameObjects to search in</param>
    /// <returns></returns>
    public static T FindComponent<T>(GameObject obj, FindMode mode = FindMode.CHILDREN_AND_SELF)
    {
        if (obj == null)
            return default(T);

        bool all = mode == FindMode.ALL;
        bool parents = all || mode == FindMode.PARENTS_AND_SELF || mode == FindMode.PARENTS;
        bool self = all || mode == FindMode.SELF || mode == FindMode.PARENTS_AND_SELF || mode == FindMode.CHILDREN_AND_SELF;
        bool children = all || mode == FindMode.CHILDREN_AND_SELF || mode == FindMode.CHILDREN;

        T comp;

        if (self)
        {
            comp = obj.GetComponent<T>();

            if (comp != null && !comp.Equals(null))
                return comp;
        }

        if (parents)
        {
            GameObject saved = obj;

            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;

                comp = obj.GetComponent<T>();

                if (comp != null && !comp.Equals(null))
                    return comp;
            }

            obj = saved;
        }

        if (children)
        {
            foreach (Transform child in obj.transform)
            {
                comp = child.gameObject.GetComponent<T>();

                if (comp != null && !comp.Equals(null))
                    return comp;
            }
        }

        return default(T);
    }

    /// <summary>
    /// Attempts to find a component in the game object, for the specific search mode (default is CHILDREN_AND_SELF).
    /// See <see cref="FindMode"/> for descriptions.
    /// </summary>
    /// <param name="mode">What GameObjects to search in</param>
    /// <returns></returns>
    public static List<T> FindComponents<T>(GameObject obj, FindMode mode = FindMode.CHILDREN_AND_SELF)
    {
        List<T> list = new List<T>();

        if (obj == null)
            return list;

        bool all = mode == FindMode.ALL;
        bool parents = all || mode == FindMode.PARENTS_AND_SELF || mode == FindMode.PARENTS;
        bool self = all || mode == FindMode.SELF || mode == FindMode.PARENTS_AND_SELF || mode == FindMode.CHILDREN_AND_SELF;
        bool children = all || mode == FindMode.CHILDREN_AND_SELF || mode == FindMode.CHILDREN;

        T comp;

        if (self)
        {
            comp = obj.GetComponent<T>();

            if (comp != null && !comp.Equals(null))
            {
                list.Add(comp);
            }
        }

        if (parents)
        {
            GameObject saved = obj;

            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;

                comp = obj.GetComponent<T>();

                if (comp != null && !comp.Equals(null))
                    list.Add(comp);
            }

            obj = saved;
        }

        if (children)
        {
            foreach (Transform child in obj.transform)
            {
                comp = child.gameObject.GetComponent<T>();

                if (comp != null && !comp.Equals(null))
                    list.Add(comp);
            }
        }

        return list;
    }
}

public static class MonoBehaviourExtentions
{
    /// <summary>
    /// Attempts to find all components in this MonoBehaviour's GameObject, for the specific search mode (default is CHILDREN_AND_SELF).
    /// See <see cref="FindMode"/> for descriptions.
    /// </summary>
    /// <param name="mode">What GameObjects to search in</param>
    /// <returns></returns>
    public static List<T> FindComponents<T>(this MonoBehaviour behaviour, RedUtil.FindMode mode = RedUtil.FindMode.CHILDREN_AND_SELF)
    {
        return RedUtil.FindComponents<T>(behaviour.gameObject, mode);
    }

    /// <summary>
    /// Attempts to find a component in this MonoBehaviour's GameObject, for the specific search mode (default is CHILDREN_AND_SELF).
    /// See <see cref="FindMode"/> for descriptions.
    /// </summary>
    /// <param name="mode">What GameObjects to search in</param>
    /// <returns></returns>
    public static T FindComponent<T>(this MonoBehaviour behaviour, RedUtil.FindMode mode = RedUtil.FindMode.CHILDREN_AND_SELF)
    {
        return RedUtil.FindComponent<T>(behaviour.gameObject, mode);
    }
}

public static class GameObjectExtentions
{
    /// <summary>
    /// Attempts to find all components in this GameObject, for the specific search mode (default is CHILDREN_AND_SELF).
    /// See <see cref="FindMode"/> for descriptions.
    /// </summary>
    /// <param name="mode">What GameObjects to search in</param>
    /// <returns></returns>
    public static List<T> FindComponents<T>(this GameObject obj, RedUtil.FindMode mode = RedUtil.FindMode.CHILDREN_AND_SELF)
    {
        return RedUtil.FindComponents<T>(obj, mode);
    }

    /// <summary>
    /// Attempts to find a component in this GameObject, for the specific search mode (default is CHILDREN_AND_SELF).
    /// See <see cref="FindMode"/> for descriptions.
    /// </summary>
    /// <param name="mode">What GameObjects to search in</param>
    /// <returns></returns>
    public static T FindComponent<T>(this GameObject obj, RedUtil.FindMode mode = RedUtil.FindMode.CHILDREN_AND_SELF)
    {
        return RedUtil.FindComponent<T>(obj, mode);
    }
}
