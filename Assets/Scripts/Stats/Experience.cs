using System;
using Saving;
using UnityEngine;

namespace Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] private float experiencePoint = 0;

        //public delegate void ExperienceGainedDelegate();
        public event Action OnExperienceGained;
        
        public void GainExperience(float experience)
        {
            experiencePoint += experience;
            OnExperienceGained?.Invoke();
        }

        public float GetExpPoints()
        {
            return experiencePoint;
        }
        
        public object CaptureState()
        {
            return experiencePoint;
        }

        public void RestoreState(object state)
        {
            experiencePoint = (float) state;
        }
    }
}