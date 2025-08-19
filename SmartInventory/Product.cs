using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventory
{
    public abstract class Product
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

        public void SetPrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new InvalidProductException("Invalid price.");
            Price = newPrice;
        }

        public void Restock(int amount)
        {
            if (amount <= 0)
                throw new InvalidProductException("Restock amount must be positive.");
            Quantity += amount;
            StockLevelChanged?.Invoke(this, $"Stock restocked for {Name}. New Qty: {Quantity}");
        }

        public bool IsLowStock()
        {
            return Quantity <= MinThreshold;
        }

        public virtual string GetDetails()
        {
            return $"ID: {ProductID}, Name: {Name}, Price: {Price:C}, Qty: {Quantity}, Min: {MinThreshold}";
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
