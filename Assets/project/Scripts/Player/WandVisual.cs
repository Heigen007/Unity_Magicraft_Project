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
    [SerializeField] private Vector3 localOffset = new Vector3(3f, 0f, 0f);

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

            // Apply initial offset only if position is at origin (not set in Inspector)
            // COMMENTED OUT - let Unity Inspector control the position
            // if (transform.localPosition == Vector3.zero)
            // {
            //     transform.localPosition = localOffset;
            // }

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
            // WandVisual больше не вращает себя!
            // Вращение теперь управляется PlayerController для разделения логики
            // (player rotation отделена от wand rotation)
            
            // Убедиться что позиция остаётся стабильной (если не установлена вручную)
            // Но НЕ перезаписывать, если позиция была изменена в Inspector
            
            // Просто убедиться что Muzzle в правильной позиции
            if (Muzzle != null)
            {
                Muzzle.localPosition = Vector3.right * tipDistance;
            }
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
