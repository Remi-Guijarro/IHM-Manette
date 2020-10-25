using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    public enum CharacterAction
    {
        DASH,
        JUMP,
        RUN
    }

    #region Particles
        [SerializeField]
        private ParticleSystem dashParticleSystemPrefab;
        private ParticleSystem dashParticleSystem;
    #endregion

    private void Awake()
    {
        dashParticleSystem = Instantiate(dashParticleSystemPrefab, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 1), Quaternion.identity);
        dashParticleSystem.transform.parent = transform;
    }

    public void StartFeedbackActionOf(CharacterAction action, float duration)
    {
        switch(action)
        {
            case CharacterAction.DASH:
                if (!dashParticleSystem.isPlaying)
                {
                    var mainModule = dashParticleSystem.main;
                    mainModule.duration = duration;
                    dashParticleSystem.Play();
                }                    
                break;
            default:
                break;
        }
    }
}
