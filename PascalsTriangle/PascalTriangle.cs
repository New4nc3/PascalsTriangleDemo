using System;
using System.Collections.Generic;

namespace PascalsTriangle
{
    // Делегат, буде використаний для генерації кастомної події поточного стану обчислень.
    public delegate void StatusUpdateHandler(object sender, ProgressEventArgs e);
    // Буде описувати поведінку (що зараз відбувається з об'єктом).
    public delegate void CurrentWorkHandler(object sender, CurrentWorkEventArgs e);

    /// <summary>
    /// Описує поведінку трикутника Паскаля.
    /// </summary>
    class PascalTriangle
    {
        // Темповський мусор.
        int _maxLevel;
        bool _textNeeded;
        string _textVar;
        List<double[]> _lstLevels;

        // Кастомна подія, реалізується для можливості підв'язки прогрес-бару.
        public event StatusUpdateHandler OnUpdateStatus;
        // Поточне завдання, яке виконує потік.
        public event CurrentWorkHandler OnTaskChanged;

        /// <summary>
        /// Створює об'єкт класу.
        /// </summary>
        /// <param name="levelNeeded">Рівень (включний), до якого будуватиметься трикутник.</param>
        /// <param name="createTextVariation">Чи потрібно формувати текстове представлення об'єкту.</param>
        public PascalTriangle(int levelNeeded = 1, bool createTextVariation = true)
        {
            SetMaxLevel(levelNeeded);

            _textNeeded = createTextVariation;
            _textVar = string.Empty;
        }

        public void SetMaxLevel(int level)
        {
            if (level <= 0) throw new ArgumentException("Try to enter positive number.");
            _maxLevel = level;
        }

        /// <summary>
        /// Обраховує трикутник за заданими параметрами конструктора.
        /// </summary>
        public void Calculate()
        {
            UpdateCurrentTask("Calculating triangle . . .");

            // Ініт списку з початковим значенням вершини.
            _lstLevels = new List<double[]> { new double[] { 1 } };

            int _tmplvl = 1;
            List<double> _tmpLst;

            while (_tmplvl < _maxLevel)
            {
                // Всі рівні починаються з одиниці.
                _tmpLst = new List<double>() { 1 };

                // Додаємо сусідні елементи попереднього рівня, йдемо до передостаннього елементу.
                for (int i = 0; i < _lstLevels[_tmplvl - 1].Length - 1; i++)
                    _tmpLst.Add(_lstLevels[_tmplvl - 1][i] + _lstLevels[_tmplvl - 1][i + 1]);

                // Останній елемент рівня теж рівний одиниці.
                _tmpLst.Add(1);

                // Додаємо рівень до загального списку, економлячи пам'ять.
                _lstLevels.Add(_tmpLst.ToArray());
                _tmplvl++;

                // Оновлення статусу виконання.
                UpdateStatus(_tmplvl);
            }

            // Генерація тектового представлення у разі необхідності.
            if (_textNeeded) GenerateTextVar();
        }

        // Створює подію зміни статусу.
        private void UpdateStatus(int tmplvl)
        {
            if (OnUpdateStatus == null) return;

            int _percentage = (int)(((double)tmplvl / (double)_maxLevel) * 100);

            ProgressEventArgs args = new ProgressEventArgs(_percentage);
            OnUpdateStatus(this, args);
        }

        // Подія зміни поточного завдання.
        private void UpdateCurrentTask(string newTask)
        {
            if (OnTaskChanged == null) return;

            CurrentWorkEventArgs arguments = new CurrentWorkEventArgs(newTask);
            OnTaskChanged(this, arguments);
        }

        // Генерує текстове представлення об'єкту.
        private void GenerateTextVar()
        {
            UpdateCurrentTask("Generating string . . .");

            _textVar = string.Empty;
            string _tmpStr;

            // Прохід по кожному рівню
            foreach (var level in _lstLevels)
            {
                //_tmpStr = string.Empty;
                _tmpStr = "\v";

                // По кожному елементу 
                foreach (var item in level)
                    //_tmpStr += $"\v\t{item}";
                    _tmpStr += $"\t{item}";
                _tmpStr += "\n";

                _textVar += _tmpStr;
            }
        }

        public override string ToString()
        {
            return _textVar.Equals(string.Empty) ? base.ToString() : _textVar;
        }
    }
}
