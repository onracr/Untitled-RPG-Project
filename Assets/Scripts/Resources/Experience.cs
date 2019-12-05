using Saving;
using UnityEngine;

namespace Resources
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] private float experiencePoint = 0;

        public void GainExperience(float experience)
        {
            experiencePoint += experience;
        }

        public object CaptureState()
        {
            return experiencePoint;
        }

        public void RestoreState(object state)
        {
            experiencePoint = (float) state;
        }

        public float GetExpPoints()
        {
            return experiencePoint;
        }
    }
}