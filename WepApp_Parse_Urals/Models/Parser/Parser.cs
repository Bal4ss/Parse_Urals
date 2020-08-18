using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApp_Parse_Urals.Models.Parser
{
    public class Parser
    {
        /// <summary>
        /// Актуальная ссылка на данные
        /// </summary>
        public static string ActualLink { get { return GetLink().Result; } }

        /// <summary>
        /// Список объектов для парсинга
        /// </summary>
        public static List<UralsItem> Items { get { return GetItem().Result; } }

        /// <summary>
        /// Метод получения актуальной ссылки на данные в json формате
        /// </summary>
        /// <returns>Ссылка на актуальную информацию</returns>
        private static async Task<string> GetLink()
        {
            //Получаем насройки парсера
            var parInfo = new ParserSettings();
            var tmp = parInfo.SearchURL;
            int index = tmp.IndexOf(parInfo.TypeOfSearch);
            if (index == -1) return null;//Проверяем корректность ссылки

            //Формирование ссылки на все версии
            var urlTmp = tmp.Substring(0, index);
            urlTmp += $"api/dataset/{tmp.Substring(index + parInfo.TypeOfSearch.Length + 1)}/version";

            //Получение актуальной версии
            var version = await new HttpClient().GetStringAsync(urlTmp + parInfo.Token);
            version = version.Split('\n').Take(3).Last().Trim().Split(':').Last().Trim().Replace("\"", null);

            return urlTmp + $"/{version}/content{parInfo.Token}";
        }

        /// <summary>
        /// Получает список объектов для парсинга
        /// </summary>
        /// <returns>Список объектов</returns>
        private static async Task<List<UralsItem>> GetItem()
        {
            //Получение актуальных данных
            string per = await new HttpClient().GetStringAsync(Parser.ActualLink);
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
    }
}
