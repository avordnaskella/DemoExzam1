using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace demka1.Models
{
    public partial class Product
    {
        // cуммарное количество на складе
        public int TotalCount => Stocks.Sum(x => x.Count);

        // текст "много" или "мало"
        public string CountString => TotalCount < 5 ? "мало" : "много";


        // флаг для подсветки true если товара мало 
        public bool IsLowStock => TotalCount <= 3;

        // проверка наличия скидки
        public bool HasDiscount
        {
            get
            {
                // вычисляем прошлый месяц
                var lastMonthDate = DateTime.Now.AddMonths(-1);
                int targetMonth = lastMonthDate.Month;
                int targetYear = lastMonthDate.Year;

                // создаем контекст, чтобы проверить заказы 
                using (var db = new PostgresContext())
                {
                    // ищем заказы на этот товар в прошлом месяце
                    bool hasOrdersInLastMonth = db.ProductsOrders
                        .Include(x => x.IdOrderNavigation) // 
                        .Any(x => x.IdProduct == this.IdProduct &&
                                  x.IdOrderNavigation.Date.Month == targetMonth &&
                                  x.IdOrderNavigation.Date.Year == targetYear);

                    // если заказов нет значит скидка есть
                    return !hasOrdersInLastMonth;
                }
            }
        }

        // итоговая цена со скидкой 25%
        public int FinalPrice => HasDiscount ? (int)(Price * 0.75) : Price;

        public Bitmap? ProductImage
        {
            get
            {
                
                if (!string.IsNullOrEmpty(Image))
                {
                    var img = LoadImage(Image);
                    if (img != null) return img;
                }

                // во всех остальных случаях
                return LoadImage("picture.png");
            }
        }

        // вспомогательный метод для загрузки картинки 
        private static Bitmap? LoadImage(string fileName)
        {
            try
            {
                var uri = new Uri($"avares://demka1/Assets/{fileName}");
                if (AssetLoader.Exists(uri))
                    return new Bitmap(AssetLoader.Open(uri));
            }
            catch { }
            return null;
        }

        // готовый цвет фона карточки
        public IBrush CardBackground => TotalCount <= 3 ? new SolidColorBrush(Color.Parse("#FFCCCC")): 
        new SolidColorBrush(Colors.White);

    }
}
