using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebApp_Parse_Urals.Models.Logger
{
    /// <summary>
    /// Модель описания операции (лога)
    /// </summary>
    public class LogInfo
    {
        private readonly Guid _logID;
        private readonly DateTime _strartTime;
        private readonly string _requestPath;
        private DateTime _endTime;

        /// <summary>
        /// Конструктор лога
        /// </summary>
        /// <param name="ctx">Контекст запроса</param>
        public LogInfo(HttpContext ctx)
        {
            _logID = Guid.NewGuid();
            _strartTime = DateTime.Now;
            _requestPath = ctx.Request.Path;
        }

        /// <summary>
        /// Возвращает время выполнения операции в миллисекундах
        /// </summary>
        private double TotalMs { get { return (_endTime - _strartTime).TotalMilliseconds; } }

        /// <summary>
        /// Устанавливает и возвращает время конца операции
        /// </summary>
        /// <returns>Время конца операции</returns>
        private DateTime SetEndTime()
        {
            _endTime = DateTime.Now;
            return _endTime;
        }

        /// <summary>
        /// Возвращает строку с информацией об операции
        /// </summary>
        /// <returns>Строка с логом</returns>
        public override string ToString()
            => $"{SetEndTime()} - " +
               $"RequestID: {_logID}; " +
               $"Complete time: {TotalMs}ms; " +
               $"Request path: {_requestPath}; ";
    }
}