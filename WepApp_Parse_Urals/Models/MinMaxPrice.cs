using System.Runtime.Serialization;

namespace WepApp_Parse_Urals.Models
{
    /// <summary>
    /// Модель с максимальным и минимальным значением выборки
    /// </summary>
    [DataContract]
    public class MinMaxPrice
    {
        private readonly double _min;
        private readonly double _max;
        public MinMaxPrice(double min, double max)
        {
            _min = min;
            _max = max;
        }

        /// <summary>
        /// Минимальное значение выборки
        /// </summary>
        [DataMember(Name = "min")]
        public double Min { get { return _min; } }

        /// <summary>
        /// Максимальное значение выборки
        /// </summary>
        [DataMember(Name = "max")]
        public double Max { get { return _max; } }
    }
}
