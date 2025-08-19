using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventory
{
    // Inheritance and Interface Implementation
    public interface IProductOperations
    {
        void UpdateStock(int change);
        bool IsLowStock();
        string GetDetails();
    }

    //Inheritance
    public abstract class Product : IProductOperations
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; private set; }
        public int MinThreshold { get; set; }

        // Event for stock changes
        public event EventHandler<string> StockLevelChanged;

        public Product(int id, string name, decimal price, int quantity, int minThreshold)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidProductException("Product name cannot be empty.");
            if (price <= 0)
                throw new InvalidProductException("Invalid price.");
            if (quantity < 0)
                throw new InvalidProductException("Invalid quantity.");
            if (minThreshold < 0)
                throw new InvalidProductException("Invalid threshold.");

            ProductID = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            MinThreshold = minThreshold;
        }

        public void UpdateStock(int change)
        {
            int newQty = Quantity + change;
            if (newQty < 0) throw new InsufficientStockException("Stock cannot go below zero.");

            Quantity = newQty;
            StockLevelChanged?.Invoke(this, $"Stock updated for {Name}. New Qty: {Quantity}");
        }

        public bool IsLowStock() => Quantity < MinThreshold;

        public virtual string GetDetails()
        {
            string status = IsLowStock() ? "LOW" : "OK";
            return $"{ProductID} | {Name} | {Price:C} | Qty: {Quantity} | Min: {MinThreshold} | {status}";
        }
    }

    public class Electronics : Product
    {
        public Electronics(int id, string name, decimal price, int quantity, int minThreshold)
            : base(id, name, price, quantity, minThreshold) { }
    }

    public class Clothing : Product
    {
        public Clothing(int id, string name, decimal price, int quantity, int minThreshold)
            : base(id, name, price, quantity, minThreshold) { }
    }

    public class Food : Product
    {
        public DateTime ExpiryDate { get; set; }

        public Food(int id, string name, decimal price, int quantity, int minThreshold, DateTime expiry)
            : base(id, name, price, quantity, minThreshold)
        {
            ExpiryDate = expiry;
        }

        public override string GetDetails()
        {
            return base.GetDetails() + $" | Exp: {ExpiryDate:yyyy-MM-dd}";
        }
    }
}
