using System.Collections.Generic;
using UnityEngine;

namespace Magicraft.Util
{
    /// <summary>
    /// Базовый пул объектов для переиспользования GameObjects
    /// Предотвращает частое создание/уничтожение объектов
    /// </summary>
    /// <typeparam name="T">Тип компонента, который будет пулиться</typeparam>
    public class ObjectPool<T> where T : Component
    {
        private readonly T prefab;
        private readonly Transform parent;
        private readonly Queue<T> availableObjects = new Queue<T>();
        private readonly List<T> allObjects = new List<T>();
        private readonly int initialSize;

        /// <summary>
        /// Конструктор пула
        /// </summary>
        /// <param name="prefab">Префаб для инстанцирования</param>
        /// <param name="parent">Родительский Transform (для организации иерархии)</param>
        /// <param name="initialSize">Начальный размер пула</param>
        public ObjectPool(T prefab, Transform parent = null, int initialSize = 10)
        {
            this.prefab = prefab;
            this.parent = parent;
            this.initialSize = initialSize;

            // Предварительное создание объектов
            Prewarm();
        }

        /// <summary>
        /// Предварительное создание объектов пула
        /// </summary>
        private void Prewarm()
        {
            for (int i = 0; i < initialSize; i++)
            {
                T instance = CreateNewInstance();
                instance.gameObject.SetActive(false);
                availableObjects.Enqueue(instance);
            }
        }

        /// <summary>
        /// Создать новый экземпляр объекта
        /// </summary>
        private T CreateNewInstance()
        {
            T instance = Object.Instantiate(prefab, parent);
            allObjects.Add(instance);
            return instance;
        }

        /// <summary>
        /// Получить объект из пула
        /// </summary>
        /// <returns>Активный объект из пула</returns>
        public T Get()
        {
            T instance;

            if (availableObjects.Count > 0)
            {
                instance = availableObjects.Dequeue();
            }
            else
            {
                // Если пул пуст, создаем новый объект
                instance = CreateNewInstance();
            }

            instance.gameObject.SetActive(true);
            return instance;
        }

        /// <summary>
        /// Вернуть объект обратно в пул
        /// </summary>
        /// <param name="instance">Объект для возврата</param>
        public void Return(T instance)
        {
            if (instance == null) return;

            instance.gameObject.SetActive(false);
            availableObjects.Enqueue(instance);
        }

        /// <summary>
        /// Очистить весь пул (уничтожить все объекты)
        /// </summary>
        public void Clear()
        {
            foreach (T obj in allObjects)
            {
                if (obj != null)
                {
                    Object.Destroy(obj.gameObject);
                }
            }

            availableObjects.Clear();
            allObjects.Clear();
        }

        /// <summary>
        /// Количество доступных объектов в пуле
        /// </summary>
        public int AvailableCount => availableObjects.Count;

        /// <summary>
        /// Общее количество созданных объектов
        /// </summary>
        public int TotalCount => allObjects.Count;
    }
}
