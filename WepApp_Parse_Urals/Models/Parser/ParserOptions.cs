namespace WebApp_Parse_Urals.Models.Parser
{
    /// <summary>
    /// Модель настроек парсера
    /// </summary>
    public class ParserOptions
    {
        private static string _url;
        private static string _type;
        private static string _token;

        /// <summary>
        /// Ссылка на источник данных
        /// </summary>
        public static string URL
        { 
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// Тип данных
        /// </summary>
        public static string Type
        { 
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Токен доступа к api
        /// </summary>
        public static string Token
        { 
            get { return $"/?access_token={_token}"; }
            set { _token = value; }
        }
    }
}
