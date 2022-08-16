using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ExclamationMark : MonoBehaviour
{
    private const string Hidden = "Hidden";
    
    private Animator _animator;

    private void Start() => _animator = GetComponent<Animator>();

    public void BeginHideAnimation()
    {
        if (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == Hidden)
            return;

        _animator.Play(Hidden);
    }
}