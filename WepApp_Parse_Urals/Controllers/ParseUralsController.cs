using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApp_Parse_Urals.Models;
using WebApp_Parse_Urals.Models.Logger;
using WebApp_Parse_Urals.Models.Parser;

namespace WebApp_Parse_Urals.Controllers
{
    [Route("api/parse")]
    [ApiController]
    [Produces("application/json")]
    public class ParseUralsController : ControllerBase
    {
        private static CustLog _logger;//Объект для подведения статистики по запросам

        /// <summary>
        /// Конструктор контроллера
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="config">Настройки проекта</param>
        public ParseUralsController(ILogger<ParseUralsController> logger, IConfiguration config)
        {
            _logger = new CustLog(logger);
            ParserOptions.URL = config.GetValue<string>("ParserOptions:URL");
            ParserOptions.Type = config.GetValue<string>("ParserOptions:Type");
            ParserOptions.Token = config.GetValue<string>("ParserOptions:Token");
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
            var log = new LogInfo(HttpContext);

            //Получение цены с учетом фильтра
            var res = Parser.Items
                .Where(c => date >= c.StartDate && date <= c.EndDate)
                .Select(c => c.Price)
                .FirstOrDefault();

            _logger.Add(log);

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
            var log = new LogInfo(HttpContext);

            //Фильтруем данные по условию
            var res = Parser.Items
                .Where(c => from <= c.StartDate && to >= c.EndDate)
                .Select(c => c.Price);

            _logger.Add(log);

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
            var log = new LogInfo(HttpContext);

            //Фильтр данных по условию
            var res = Parser.Items
                .Where(c => from <= c.StartDate && to >= c.EndDate)
                .Select(c => c.Price);

            //Вычисление минимального и максимального значения цены в выборке
            double Min = res.Count() > 0 ? res.Min() : 0;
            double Max = res.Count() > 0 ? res.Max() : 0;

            _logger.Add(log);

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
            var log = new LogInfo(HttpContext);
            var res = Parser.Items;

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

            _logger.Add(log);

            //Возвращение результата вычисления
            return new DataStatistic(Count, minMaxValue, minDate, maxDate, res.Average(c => c.Price));
        }

        /// <summary>
        /// Вывод лога в api 
        /// Пример запроса: {ссылка}/api/parse/GetLog
        /// </summary>
        /// <returns>Список логов</returns>
        [HttpGet("GetLog")]
        public List<string> GetLog()
        {
            _logger.Add(new LogInfo(HttpContext));
            return _logger.Log.Select(c=>c.ToString()).ToList();
        }
    }
}
