using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApp_Parse_Urals.Models;

namespace WebApp_Parse_Urals.Controllers
{
    [Route("api/parse")]
    [ApiController]
    [Produces("application/json")]
    public class ParseUralsController : ControllerBase
    {
        private static ILogger<ParseUralsController> _logger;//Объект для подведения статистики по запросам

        /// <summary>
        /// Конструктор контроллера
        /// </summary>
        /// <param name="logger"></param>
        public ParseUralsController(ILogger<ParseUralsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Метод получения актуальной ссылки на данные в json формате
        /// </summary>
        /// <returns>Ссылка на актуальную информацию</returns>
        private async Task<string> GetLink()
        {
            //Получаем насройки парсера
            var parInfo = new ParserSettings();
            var tmp = parInfo.SearchURL;
            int index = tmp.IndexOf(parInfo.TypeOfSearch);
            if (index == -1) return null;//Проверяем корректность ссылки

            //Формирование ссылки на все версии
            var urlTmp = tmp.Substring(0, index);
            urlTmp += $"api/dataset/{tmp.Substring(index+ parInfo.TypeOfSearch.Length+1)}/version";

            //Получение актуальной версии
            var version = await new HttpClient().GetStringAsync(urlTmp + parInfo.Token);
            version = version.Split('\n').Take(3).Last().Trim().Split(':').Last().Trim().Replace("\"",null);

            return urlTmp + $"/{version}/content{parInfo.Token}";
        }

        /// <summary>
        /// Получает список объектов для парсинга
        /// </summary>
        /// <returns>Список объектов</returns>
        private async Task<List<UralsItem>> GetItem()
        {
            //Получение актуальных данных
            string per = await new HttpClient().GetStringAsync(GetLink().Result);
            var splitChars = new string[] { "{", "[", "]" };//Разделители
            var uralItem = new List<UralsItem>();

            //Обработка и форматирование данных в удобный для обработки формат
            foreach (var str in per.Split("},"))//Разделение данных на сегменты
            {
                var res = str.Split('\n')//Разделение данных на строчки с необходимой информацией
                    .Select(c => c.Trim().Replace("\"", ""))//Исключение лишних символов
                    .Where(c => !splitChars.Contains(c) && !String.IsNullOrEmpty(c))//Валидация данных
                    .Select(c => c.Split(':').Last().Trim())//Получение данных
                    .ToArray();
                uralItem.Add(new UralsItem(res[0], res[1], res[2]));
            }
            return uralItem;
        }

        /// <summary>
        /// Логирование запроса
        /// </summary>
        /// <param name="time">Время начала операции</param>
        private void SetLog(DateTime time)
        {
            _logger.Log(LogLevel.Information, $"{DateTime.Now} - \n" +
                $"Complete time: {(DateTime.Now - time).TotalMilliseconds}ms; \n" +
                $"Request path: {HttpContext.Request.Path}; ");
        }

        /// <summary>
        /// Цена нефти на заданную дату
        /// Пример запроса: {ссылка}/api/parse/GetPriceFromDate/?date=2016-12-05
        /// </summary>
        /// <param name="date">Дата для фильтра</param>
        /// <returns>Значение цены</returns>
        [HttpGet("GetPriceFromDate/")]
        public double? GetPrice(DateTime date)
        {
            var time = DateTime.Now;

            //Получение цены с учетом фильтра
            var res = GetItem().Result
                .Where(c => date >= c.StartDate && date <= c.EndDate)
                .Select(c => c.Price)
                .FirstOrDefault();

            SetLog(time);

            //Возвращение результата вычисления
            return res;
        }

        /// <summary>
        /// Средняя цена нефти за промежуток времени
        /// Пример запроса: {ссылка}/api/parse/GetAvgPriceFromDateToDate/?from=2013-03-15&to=2013-05-14
        /// </summary>
        /// <param name="from">Дата начала диапазона дат</param>
        /// <param name="to">Дата конца диапазона дат</param>
        /// <returns>Среднее значение цены</returns>
        [HttpGet("GetAvgPriceFromDateToDate/")]
        public double? GetAvg(DateTime from, DateTime to)
        {
            var time = DateTime.Now;

            //Фильтруем данные по условию
            var res = GetItem().Result
                .Where(c => from <= c.StartDate && to >= c.EndDate)
                .Select(c => c.Price);

            SetLog(time);

            //Если результат фильтрации не является нулевым, вычисляется среднее значение, иначе 0
            return res.Count() > 0 ? Math.Round(res.Average(), 2) : 0;
        }

        /// <summary>
        /// Максимальная и минимальная цены на нефть за промежуток времени
        /// Пример запроса: {ссылка}/api/parse/GetMinMaxPriceFromDateToDate/?from=2013-03-15&to=2013-05-14
        /// </summary>
        /// <param name="from">Дата начала диапазона дат</param>
        /// <param name="to">Дата конца диапазона дат</param>
        /// <returns>Json строку с полями min и max</returns>
        [HttpGet("GetMinMaxPriceFromDateToDate/")]
        public MinMaxPrice GetMinMax(DateTime from, DateTime to)
        {
            var time = DateTime.Now;

            //Фильтр данных по условию
            var res = GetItem().Result
                .Where(c => from <= c.StartDate && to >= c.EndDate)
                .Select(c => c.Price);

            //Вычисление минимального и максимального значения цены в выборке
            double Min = res.Count() > 0 ? res.Min() : 0;
            double Max = res.Count() > 0 ? res.Max() : 0;

            SetLog(time);

            //Возвращение результата вычисления
            return new MinMaxPrice(Min, Max);
        }

        /// <summary>
        /// Статистика по загруженным данным
        /// Пример запроса: {ссылка}/api/parse/GetStat
        /// </summary>
        /// <returns>
        /// AllAvgPrice - среднее значение цены по всей выборке
        /// </returns>
        [HttpGet("GetStat")]
        public DataStatistic GetStat()
        {
            var time = DateTime.Now;
            var res = GetItem().Result;

            //Получение количества записей в выборке
            var Count = res.Count;

            //Проверка на пустоту и получение минимального и максимального значения цены в выборке
            var minMaxValue = Count > 0 ?
                new MinMaxPrice(res.Select(c => c.Price).Min(), res.Select(c => c.Price).Max()) :
                new MinMaxPrice(0, 0);

            //Получение стартовой даты минимального значения цены
            var minDate = res.Where(c => c.Price == minMaxValue.Min)
                .Select(c=>c.StartDate)
                .FirstOrDefault();

            //Получение стартовой даты максимального значения цены
            var maxDate = res.Where(c => c.Price == minMaxValue.Max)
                .Select(c => c.StartDate)
                .FirstOrDefault();

            //Проверка на пустоту и получение среднего значения цены в выборке
            var avgPrice = Count > 0 ? res.Average(c => c.Price) : 0;

            SetLog(time);

            //Возвращение результата вычисления
            return new DataStatistic(Count, minMaxValue, minDate, maxDate, res.Average(c => c.Price));
        }
    }
}
