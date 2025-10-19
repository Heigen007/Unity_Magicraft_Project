using UnityEngine;
using Magicraft.Combat;

namespace Magicraft.Player
{
    /// <summary>
    /// Контроллер для интеграции Wand с системой ввода
    /// Автоматически вызывает каст при удержании кнопки огня
    /// </summary>
    [RequireComponent(typeof(Wand))]
    [RequireComponent(typeof(PlayerInputHandler))]
    public class WandInputController : MonoBehaviour
    {
        private Wand wand;
        private PlayerInputHandler inputHandler;

        private void Awake()
        {
            wand = GetComponent<Wand>();
            inputHandler = GetComponent<PlayerInputHandler>();
        }

        private void Update()
        {
            // Авто-каст при удержании ЛКМ
            if (inputHandler != null && inputHandler.IsFireHeld)
            {
                if (wand != null && wand.IsReady)
                {
                    wand.TryCast();
                }
            }
        }
    }
}
