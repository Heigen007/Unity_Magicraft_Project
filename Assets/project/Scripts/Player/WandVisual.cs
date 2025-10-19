using UnityEngine;
using Magicraft.Combat;

namespace Magicraft.Player
{
    /// <summary>
    /// Простая визуальная палка (посох).
    /// - Поворачивает объект к AimDirection из ICaster (обычно PlayerController)
    /// - Хранит дочерний Transform "Muzzle" в качестве точки спавна снарядов
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class WandVisual : MonoBehaviour
    {
        [Tooltip("(Optional) Указать компонент, реализующий ICaster (например PlayerController). Если пусто — будет попытка найти на родителе.)")]
        [SerializeField] private MonoBehaviour casterComponent;

    [Tooltip("Локальное смещение визуала относительно игрока (в дочернем пространстве)")]
    [SerializeField] private Vector3 localOffset = new Vector3(0.5f, 0f, 0f);

    [Tooltip("Расстояние от рукояти до кончика посоха в локальных единицах (настройка для точного размещения)")]
    [SerializeField] private float tipDistance = 0.8f;

        // Public Muzzle transform (assignable or created automatically)
        public Transform Muzzle { get; private set; }

    private ICaster caster;
    private SpriteRenderer spriteRenderer;
    private Transform spriteTransform;

        private void Awake()
        {
            // Resolve caster
            if (casterComponent != null && casterComponent is ICaster)
            {
                caster = casterComponent as ICaster;
            }
            else
            {
                // Try to find ICaster on the same GameObject or parents
                caster = GetComponentInParent<ICaster>();
            }

            // Find or create Muzzle child (use tipDistance to place it)
            var muzzleChild = transform.Find("Muzzle");
            if (muzzleChild != null)
            {
                Muzzle = muzzleChild;
            }
            else
            {
                GameObject m = new GameObject("Muzzle");
                m.transform.SetParent(transform, false);
                m.transform.localPosition = Vector3.right * tipDistance;
                Muzzle = m.transform;
            }

            // Apply initial offset (holder offset relative to player)
            transform.localPosition = localOffset;

            // Find SpriteRenderer (optional)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteTransform = spriteRenderer.transform;
                // Place sprite child so its center is at half of tipDistance to the right
                spriteTransform.localPosition = Vector3.right * (tipDistance * 0.5f);
            }
        }

        private void Update()
        {
            if (caster == null) return;

            Vector2 aim = caster.AimDirection;
            if (aim.sqrMagnitude < 0.0001f) return;

            float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;

            // Set world rotation so holder faces the aim direction; this keeps base anchored
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Ensure muzzle stays at tipDistance in local space
            Muzzle.localPosition = Vector3.right * tipDistance;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * 0.5f);
            Gizmos.DrawSphere(Muzzle != null ? Muzzle.position : transform.position + transform.right * 0.5f, 0.05f);
        }
#endif
    }
}
