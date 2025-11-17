using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicraft.Rewards
{
    /// <summary>
    /// Система наград: отслеживает убийства и показывает выбор наград
    /// </summary>
    public class RewardSystem : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Количество убийств для получения награды")]
        [SerializeField] private int killsPerReward = 10;

        [Tooltip("Количество вариантов награды")]
        [SerializeField] private int optionsCount = 3;

        [Header("References")]
        [Tooltip("UI выбора наград")]
        [SerializeField] private RewardChoiceUI rewardChoiceUI;

        [Tooltip("Посох игрока для добавления наград")]
        [SerializeField] private Combat.Wand playerWand;

        [Header("Reward Pool")]
        [Tooltip("Доступные заклинания для наград")]
        [SerializeField] private List<ScriptableObject> availableSpells = new List<ScriptableObject>();

        [Tooltip("Доступные бафы для наград")]
        [SerializeField] private List<ScriptableObject> availableBuffs = new List<ScriptableObject>();

        // Текущее состояние
        private int totalKills = 0;
        private int nextRewardKills = 10;

        // События
        public event Action<int> OnKillCountChanged; // (totalKills)
        public event Action<RewardOption> OnRewardChosen; // (chosenReward)

        private void Awake()
        {
            // Найти посох автоматически, если не задан
            if (playerWand == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerWand = player.GetComponent<Combat.Wand>();
                }
            }

            // Найти UI автоматически, если не задан
            if (rewardChoiceUI == null)
            {
                rewardChoiceUI = FindFirstObjectByType<RewardChoiceUI>();
            }

            // Подписка на события UI
            if (rewardChoiceUI != null)
            {
                rewardChoiceUI.OnRewardSelected += HandleRewardChosen;
            }
        }

        private void Start()
        {
            // Подписка на static событие смерти врагов
            Combat.EnemyController.OnAnyEnemyKilled += OnEnemyKilled;
        }

        /// <summary>
        /// Обработка убийства врага
        /// </summary>
        private void OnEnemyKilled(GameObject killer)
        {
            // Проверка, что убийца - игрок
            if (killer == null) return;
            if (!killer.CompareTag("Player")) return;

            // Увеличить счётчик
            totalKills++;
            OnKillCountChanged?.Invoke(totalKills);

            Debug.Log($"[RewardSystem] Kill #{totalKills}");

            // Проверка на награду
            if (totalKills >= nextRewardKills)
            {
                ShowRewardChoice();
                nextRewardKills += killsPerReward;
            }
        }

        /// <summary>
        /// Показать выбор наград
        /// </summary>
        private void ShowRewardChoice()
        {
            Debug.Log($"[RewardSystem] Showing reward choice after {totalKills} kills");

            // Генерация вариантов
            List<RewardOption> options = GenerateRewardOptions(optionsCount);

            // Показать UI
            if (rewardChoiceUI != null)
            {
                rewardChoiceUI.ShowRewards(options);
            }
            else
            {
                Debug.LogError("[RewardSystem] RewardChoiceUI not found!");
            }
        }

        /// <summary>
        /// Генерация случайных вариантов наград
        /// </summary>
        private List<RewardOption> GenerateRewardOptions(int count)
        {
            List<RewardOption> options = new List<RewardOption>();

            for (int i = 0; i < count; i++)
            {
                // Случайный выбор: Spell или Buff (50/50)
                bool isSpell = UnityEngine.Random.value > 0.5f;

                if (isSpell && availableSpells.Count > 0)
                {
                    var spell = availableSpells[UnityEngine.Random.Range(0, availableSpells.Count)];
                    options.Add(RewardOption.CreateSpell(spell));
                }
                else if (availableBuffs.Count > 0)
                {
                    var buff = availableBuffs[UnityEngine.Random.Range(0, availableBuffs.Count)];
                    options.Add(RewardOption.CreateBuff(buff));
                }
                else
                {
                    Debug.LogWarning("[RewardSystem] No rewards available to generate!");
                }
            }

            return options;
        }

        /// <summary>
        /// Обработка выбора награды
        /// </summary>
        private void HandleRewardChosen(RewardOption reward)
        {
            if (!reward.IsValid())
            {
                Debug.LogError("[RewardSystem] Invalid reward chosen!");
                return;
            }

            Debug.Log($"[RewardSystem] Player chose: {reward.GetName()} ({reward.type})");

            // Добавить в посох
            if (playerWand != null)
            {
                if (reward.IsSpell)
                {
                    playerWand.AddSlot(reward.Data, Combat.SlotType.Spell);
                }
                else if (reward.IsBuff)
                {
                    playerWand.AddSlot(reward.Data, Combat.SlotType.Buff);
                }
            }
            else
            {
                Debug.LogError("[RewardSystem] Player wand not found!");
            }

            OnRewardChosen?.Invoke(reward);
        }

        /// <summary>
        /// Получить текущий прогресс к следующей награде
        /// </summary>
        public float GetProgressToNextReward()
        {
            int killsSinceLastReward = totalKills % killsPerReward;
            return (float)killsSinceLastReward / killsPerReward;
        }

        /// <summary>
        /// Получить количество убийств до следующей награды
        /// </summary>
        public int GetKillsUntilNextReward()
        {
            return nextRewardKills - totalKills;
        }

        #region Debug Methods
        [ContextMenu("Debug: Grant Reward Now")]
        private void DebugGrantReward()
        {
            ShowRewardChoice();
        }

        [ContextMenu("Debug: Add 5 Kills")]
        private void DebugAdd5Kills()
        {
            for (int i = 0; i < 5; i++)
            {
                totalKills++;
            }
            OnKillCountChanged?.Invoke(totalKills);
            Debug.Log($"[RewardSystem] DEBUG: Added 5 kills. Total: {totalKills}");

            if (totalKills >= nextRewardKills)
            {
                ShowRewardChoice();
                nextRewardKills += killsPerReward;
            }
        }
        #endregion

        private void OnDestroy()
        {
            if (rewardChoiceUI != null)
            {
                rewardChoiceUI.OnRewardSelected -= HandleRewardChosen;
            }

            // Отписка от static события
            Combat.EnemyController.OnAnyEnemyKilled -= OnEnemyKilled;
        }
    }
}
