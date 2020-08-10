namespace WepApp_Parse_Urals.Models
{
    /// <summary>
    /// Настройки парсера / Parser settings
    /// </summary>
    public class ParserSettings
    {
        private readonly string _searchUrl = "https://data.gov.ru/opendata/7710349494-urals";
        private readonly string _type = "opendata";
        private readonly string _token = "80f009db8264e10369004be939ccbdbd";
        /// <summary>
        /// Стандатная ссылка для парсинга / Base search link
        /// </summary>
        public string SearchURL { get { return _searchUrl; } }
        /// <summary>
        /// Ключ доступа к открытым данным
        /// </summary>
        public string Token { get { return $"/?access_token={_token}"; } }
        public string TypeOfSearch { get { return _type; } }
    }
}
