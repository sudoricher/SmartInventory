using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventory
{
    public class InventoryManager
    {
        private readonly List<Product> products = new List<Product>();
        private int nextId = 1;

        // Delegate & Event for low stock
        public delegate void LowStockHandler(Product product);
        public event LowStockHandler LowStockAlert;

        public void AddProduct(Product p)
        {
            products.Add(p);
            p.StockLevelChanged += OnStockLevelChanged;
            if (p.IsLowStock())
                LowStockAlert?.Invoke(p);
        }

        public Product FindById(int id)
        {
            return products.Find(p => p.ProductID == id);
        }

        public void ViewInventory()
        {
            if (products.Count == 0)
            {
                Console.WriteLine("No products.");
                return;
            }
            foreach (var p in products)
                Console.WriteLine(p.GetDetails());
        }

        public void ShowLowStock()
        {
            foreach (var p in products)
                if (p.IsLowStock())
                    Console.WriteLine(p.GetDetails());
        }

        public int GetNextId() => nextId++;

        private void OnStockLevelChanged(object sender, string msg)
        {
            Console.WriteLine("[EVENT] " + msg);
        }

        // Expose product list for monitor
        public List<Product> GetProducts() => products;

        // Add this method to your InventoryManager class
        public void RaiseLowStockAlert(Product product)
        {
            LowStockAlert?.Invoke(product);
        }
    }
}
