using UnityEngine;
using UnityEngine.UI;

namespace UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private Text damageText = null;

        public void SetValue(float amount)
        {
            damageText.text = $"{ amount }";
        }
    }
}
