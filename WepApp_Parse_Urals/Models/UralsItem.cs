using System;
using System.Runtime.Serialization;

namespace WepApp_Parse_Urals.Models
{
    /// <summary>
    /// Модель экземпляра выборки
    /// </summary>
    [DataContract]
    public class UralsItem
    {
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;
        private readonly double _price;

        /// <summary>
        /// Конструктор модели
        /// </summary>
        /// <param name="startDate">Дата начала периодамониторинга цен на нефть</param>
        /// <param name="endDate">Дата конца периодамониторинга цен на нефть</param>
        /// <param name="price">Цена на нефть</param>
        public UralsItem(string startDate, string endDate, string price)
        {
            _startDate = Convert.ToDateTime(startDate.Replace(",",""));
            _endDate = Convert.ToDateTime(endDate.Replace(",", ""));
            if (!double.TryParse(price, out _price)) 
                _price = 0;
        }

        /// <summary>
        /// Возвращает дату начала периодамониторинга цен на нефть
        /// </summary>
        [DataMember(Name = "startDate")]
        public DateTime StartDate { get { return _startDate; } }

        /// <summary>
        /// Возвращает дату конца периодамониторинга цен на нефть
        /// </summary>
        [DataMember(Name = "endDate")]
        public DateTime EndDate { get { return _endDate; } }

        /// <summary>
        /// Возвращает значение средней цены на нефть сырую марки «Юралс» на мировых рынках нефтяного сырья (средиземноморском и роттердамском)
        /// </summary>
        [DataMember(Name = "price")]
        public double Price { get { return _price; } }
    }
}
