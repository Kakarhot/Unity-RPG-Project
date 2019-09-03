using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform foreGround = null;

        Health healthComponent;
        Canvas canvas;

        private void Awake() {
            healthComponent = GetComponentInParent<Health>();
            canvas = GetComponentInChildren<Canvas>();
            canvas.enabled = false;
        }

        private void Update() {
            if (healthComponent.GetPercentage() < 100 && healthComponent.GetPercentage() > 0)
            {
                canvas.enabled = true;
                foreGround.localScale = new Vector3(healthComponent.GetPercentage() / 100, 1, 1);
            }           
            else
            {
                canvas.enabled = false;
            }
        }
    }
}
