using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    [SerializeField] private ParticleSystem _effectPrefab;

    private List<ParticleSystem> _pool = new List<ParticleSystem>();
    private int _lastGivenIndex;

    public ParticleSystem GetEffect()
    {
        var particle = _pool.FirstOrDefault(particle => !particle.isPlaying);

        if (!particle)
        {
            var newParticle = Instantiate(_effectPrefab);
            var main = newParticle.main;
            main.stopAction = ParticleSystemStopAction.None;
            newParticle.Play();
            _pool.Add(newParticle);
            
            return newParticle;
        }

        particle.Play();
        return particle;
    }
}