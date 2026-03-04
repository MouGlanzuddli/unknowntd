using UnityEngine;

namespace Enemy
{
    public class EnemyHPBar : MonoBehaviour
    {
        [SerializeField] private Transform fillTransform;
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private bool useSmoothTransition = true;

        private float maxHP;
        private float targetScale = 1f;

        public void Init(float maxHP)
        {
            this.maxHP = maxHP;
            SetHP(maxHP);
        }

        public void SetHP(float currentHP)
        {
            float percent = Mathf.Clamp01(currentHP / maxHP);
            targetScale = percent;

            if (!useSmoothTransition)
            {
                fillTransform.localScale = new Vector3(percent, 1f, 1f);
            }
        }

        private void Update()
        {
            if (!useSmoothTransition) return;

            float currentScale = fillTransform.localScale.x;
            float newScale = Mathf.Lerp(currentScale, targetScale, smoothSpeed * Time.deltaTime);
            fillTransform.localScale = new Vector3(newScale, 1f, 1f);
        }
    }
}
