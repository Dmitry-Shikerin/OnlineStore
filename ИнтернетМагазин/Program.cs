using System;
using System.Collections.Generic;

namespace ИнтернетМагазин
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Good iPhone12 = new Good("IPhone 12");
            Good iPhone11 = new Good("IPhone 11");

            Warehouse warehouse = new Warehouse();

            Shop shop = new Shop(warehouse);

            warehouse.Delive(iPhone12, 10);
            warehouse.Delive(iPhone11, 1);

            //Вывод всех товаров на складе с их остатком

            Cart cart = shop.Cart();
            cart.Add(iPhone12, 4);
            cart.Add(iPhone11, 3); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе

            //Вывод всех товаров в корзине

            Console.WriteLine(cart.Order().Paylink);

            cart.Add(iPhone12, 9); //Ошибка, после заказа со склада убираются заказанные товары
        }
    }

    class Good
    {
        public Good(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    interface IWarehouse
    {
        bool CheckAvailability(Good good, int quantity);
        void Delete(Good good, int quantity);
    }

    class Warehouse : IWarehouse
    {
        private Dictionary<Good, int> _goods = new Dictionary<Good, int>();

        public void Show()
        {
            foreach (var good in _goods)
            {
                Console.WriteLine($"Товар - {good.Key.Name}, количество - {good.Value}");
            }
        }

        public bool CheckAvailability(Good good, int quantity)
        {
            if (_goods.TryGetValue(good, out int count))
            {
                if (count < quantity)
                {
                    return false;
                }
            }

            return true;
        }

        public void Delete(Good good, int quantity)
        {
            if (CheckAvailability(good, quantity) == false)
                throw new InvalidOperationException();

            _goods[good] -= quantity;

            if (_goods[good] != 0)
                return;

            _goods.Remove(good);
        }

        public void Delive(Good good, int quantity)
        {
            if (_goods.ContainsKey(good) == false)
            {
                _goods[good] = 0;
            }

            _goods[good] += quantity;

            Show();
        }
    }

    class Cart
    {
        private Dictionary<Good, int> _goods = new Dictionary<Good, int>();

        private IWarehouse _warehouse;

        public Cart(IWarehouse warehouse)
        {
            _warehouse = warehouse;
        }

        public void Show()
        {
            foreach (var good in _goods)
            {
                Console.WriteLine($"Товар - {good.Key.Name}, количество - {good.Value}");
            }
        }

        public void Add(Good good, int quantity)
        {
            if (_warehouse.CheckAvailability(good, quantity) == false)
                throw new InvalidOperationException();

            if (_goods.ContainsKey(good) == false)
            {
                _goods[good] = 0;
            }

            _goods[good] += quantity;

            Show();
        }

        public Order Order()
        {
            foreach (KeyValuePair<Good, int> keyValuePair in _goods)
            {
                _warehouse.Delete(keyValuePair.Key, keyValuePair.Value);
            }

            return new Order(_goods, "Ссылка для оплаты");
        }
    }

    class Order
    {
        private Dictionary<Good, int> _goods = new Dictionary<Good, int>();

        public Order(Dictionary<Good, int> goods, string paylink)
        {
            _goods = goods;
            Paylink = paylink;
        }

        public string Paylink { get; }
    }

    class Shop
    {
        private IWarehouse _warehouse;
        private Cart _cart;

        public Shop(IWarehouse warehouse)
        {
            _warehouse = warehouse;
        }

        public Cart Cart()
        {
            return new Cart(_warehouse);
        }
    }
}
