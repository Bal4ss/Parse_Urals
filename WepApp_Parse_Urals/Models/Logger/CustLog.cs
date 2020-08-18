using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using WebApp_Parse_Urals.Controllers;

namespace WebApp_Parse_Urals.Models.Logger
{
    /// <summary>
    /// Модель списка логов
    /// </summary>
    public class CustLog
    {
        private static List<LogInfo> _logList = new List<LogInfo>();
        private readonly ILogger<ParseUralsController> _logger;

        /// <summary>
        /// Конструктор модели
        /// </summary>
        /// <param name="logger">Логгер</param>
        public CustLog(ILogger<ParseUralsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Возвращает список логов
        /// </summary>
        public List<LogInfo> Log { get { return _logList; } }

        /// <summary>
        /// Добавляет новый лог
        /// </summary>
        /// <param name="log">Новый лог на добавление</param>
        public void Add(LogInfo log)
        {
            _logger.Log(LogLevel.Information, log.ToString());
            _logList.Add(log);
        }
    }
}
