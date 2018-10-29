using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class Fan : MonoBehaviour
{
    [SerializeField]
    private bool startEnabled = false;

    private void Start()
    {
        isRotating = startEnabled;
    }

    private bool _enabled = false;

    public bool isRotating
    {
        get
        {
            return _enabled;
        }

        set
        {
            Animator animator = GetComponent<Animator>();
            animator.Play(value ? "Rotate" : "Idle");
        }
    }

}
