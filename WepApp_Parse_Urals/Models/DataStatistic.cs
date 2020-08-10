using System;

namespace WepApp_Parse_Urals.Models
{
    /// <summary>
    /// Модель статистики по выборке
    /// </summary>
    public class DataStatistic
    {
        private readonly int _rowCount;
        private readonly MinMaxPrice _minMaxValue;
        private readonly DateTime _minDate;
        private readonly DateTime _maxDate;
        private readonly double _avgValue;

        /// <summary>
        /// Конструктор модели
        /// </summary>
        /// <param name="rowCount">Количество записей в выборке</param>
        /// <param name="minValue">Минимальноей значение цены в выборки</param>
        /// <param name="minDate">Стартовая дата минимального значения</param>
        /// <param name="maxValue">Максимальное значение цены в выборки</param>
        /// <param name="maxDate">Стартовая дата максимального значения</param>
        /// <param name="avgValue">Среднее значение цены в выборке</param>
        public DataStatistic(int rowCount, MinMaxPrice minMaxValue, DateTime minDate,
             DateTime maxDate, double avgValue)
        {
            _rowCount = rowCount;
            _minMaxValue = minMaxValue;
            _minDate = minDate;
            _maxDate = maxDate;
            _avgValue = Math.Round(avgValue, 2);
        }

        /// <summary>
        /// Возвращает значение количества записей в выборке
        /// </summary>
        public int RowCount { get { return _rowCount; } }

        /// <summary>
        /// Возвращает значение минимальной и максимальной цены в выборке
        /// </summary>
        public MinMaxPrice MinMaxValue { get { return _minMaxValue; } }

        /// <summary>
        /// Возвращает стартовую дату минимального значения в выборке
        /// </summary>
        public string MinDate { get { return _minDate.ToShortDateString(); } }

        /// <summary>
        /// Возвращает стартовую дату максимального значения в выборке
        /// </summary>
        public string MaxDate { get { return _maxDate.ToShortDateString(); } }

        /// <summary>
        /// Возвращает значение средней цены в выборке
        /// </summary>
        public double AvgValue { get { return _avgValue; } }
    }
}
