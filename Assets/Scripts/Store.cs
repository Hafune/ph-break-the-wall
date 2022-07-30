using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Store : MonoBehaviour
{
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");

    [SerializeField] private Hands _hands;
    [SerializeField] private TextMeshProUGUI _speedText;
    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private TextMeshProUGUI _speedCardText;
    [SerializeField] private TextMeshProUGUI _powerCardText;
    [SerializeField] private String _speedCardPattern;
    [SerializeField] private String _powerCardPattern;

    private Animator _animator;

    public void SetOpen(bool isOpen)
    {
        _animator.SetBool(IsOpen, isOpen);
        UpdateText();
    }

    public void TryBuyAttackSpeed()
    {
        _hands.AddAttackSpeed();
        UpdateText();
    }

    public void TryBuyHitPower()
    {
        _hands.AddHitPower();
        UpdateText();
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _speedCardPattern = _speedCardText.text;
        _powerCardPattern = _powerCardText.text;
        UpdateText();
    }

    private void UpdateText()
    {
        _speedText.text = _hands.AttackSpeed.ToString();
        _powerText.text = _hands.HitPower.ToString();
        _speedCardText.text = string.Format(_speedCardPattern, _hands.AttackSpeed + 1);
        _powerCardText.text = string.Format(_powerCardPattern, _hands.HitPower + 1);
    }
}