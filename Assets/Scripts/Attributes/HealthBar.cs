using UnityEngine;

namespace Attributes
{
    class HealthBar : MonoBehaviour
    {
        [SerializeField] private Canvas healthBarCanvas = null;
        [SerializeField] private RectTransform healthBar = null;

        private Health _healthComponent = null;

        private void Awake()
        {
            _healthComponent = GetComponentInParent<Health>();
        }

        private void Update()
        {
            if (GetApproximation(0) || GetApproximation(1)) 
            {
                healthBarCanvas.enabled = false;
                return;
            }

            healthBarCanvas.enabled = true;
            healthBar.localScale = new Vector3(_healthComponent.GetFraction(), 1, 1);
        }

        private bool GetApproximation(float rhv)
        {
            return Mathf.Approximately(_healthComponent.GetFraction(), rhv);
        }
    }
}
