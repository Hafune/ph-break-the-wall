using System.Collections.Generic;
using UnityEngine;

public class LevelProgress : MonoBehaviour
{
    private static readonly int InProgress = Animator.StringToHash("InProgress");
    private static readonly int Completed = Animator.StringToHash("Completed");

    [SerializeField] private List<Animator> _pipes;
    [SerializeField] private int _currentLevel;

    public void UnlockNext() => _pipes[_currentLevel++].SetTrigger(InProgress);

    private void Start()
    {
        for (int i = 0; i < _currentLevel; i++)
            _pipes[i].SetTrigger(Completed);
    }
}