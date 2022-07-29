using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Store : MonoBehaviour
{
    private Animator _animator;
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");
    
    public void SetOpen(bool isOpen) => _animator.SetBool(IsOpen,isOpen);

    private void Start() => _animator =GetComponent<Animator>();
}
