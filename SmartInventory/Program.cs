using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventory
{
    class Program
    {
        static void Main(string[] args)
        {
            var security = new SecurityManager();
            var inventory = new InventoryManager();

            // Subscribe to low stock alerts
            inventory.LowStockAlert += (p) =>
            {
                Console.WriteLine($"[ALERT] Low stock on {p.Name}! Current: {p.GetDetails()}");
            };

            // Authentication
            bool loggedIn = false;
            while (!loggedIn)
            {
                Console.Write("Username: ");
                string user = Console.ReadLine();
                Console.Write("Password: ");
                string pass = Console.ReadLine();

                if (security.Authenticate(user, pass))
                {
                    Console.WriteLine($"Welcome {user} ({security.CurrentUserRole})");
                    loggedIn = true;
                }
                else
                {
                    Console.WriteLine("Invalid login, try again.");
                }
            }

            // Start background monitor
            var monitor = new InventoryMonitor(inventory);
            monitor.Start();

            // Main menu
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n--- MENU ---");
                Console.WriteLine("1. View Inventory");
                Console.WriteLine("2. Add Product");
                Console.WriteLine("3. Update Stock");
                Console.WriteLine("4. Show Low Stock");
                Console.WriteLine("5. Exit");
                Console.Write("Choice: ");

                string choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            if (security.HasPermission("View"))
                                inventory.ViewInventory();
                            else Console.WriteLine("Access denied.");
                            break;

                        case "2":
                            if (security.HasPermission("Add"))
                            {
                                Console.WriteLine("1) Electronics 2) Clothing 3) Food");
                                string type = Console.ReadLine();
                                Console.Write("Name: "); string name = Console.ReadLine();
                                Console.Write("Price: "); decimal price = decimal.Parse(Console.ReadLine());
                                Console.Write("Qty: "); int qty = int.Parse(Console.ReadLine());
                                Console.Write("Min: "); int min = int.Parse(Console.ReadLine());

                                Product p;
                                if (type == "1")
                                {
                                    p = new Electronics(inventory.GetNextId(), name, price, qty, min);
                                }
                                else if (type == "2")
                                {
                                    p = new Clothing(inventory.GetNextId(), name, price, qty, min);
                                }
                                else if (type == "3")
                                {
                                    p = new Food(inventory.GetNextId(), name, price, qty, min, DateTime.Today.AddDays(7));
                                }
                                else
                                {
                                    throw new InvalidProductException("Invalid type");
                                }
                                inventory.AddProduct(p);
                            }
                            else Console.WriteLine("Access denied.");
                            break;

                        case "3":
                            if (security.HasPermission("Update"))
                            {
                                Console.Write("Enter ID: ");
                                int id = int.Parse(Console.ReadLine());
                                Product p = inventory.FindById(id);
                                if (p == null) throw new InvalidProductException("Product not found.");
                                Console.Write("Change (+/-): ");
                                int change = int.Parse(Console.ReadLine());
                                p.UpdateStock(change);
                            }
                            else Console.WriteLine("Access denied.");
                            break;

                        case "4":
                            inventory.ShowLowStock();
                            break;

                        case "5":
                            running = false;
                            monitor.Stop();
                            break;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] " + ex.Message);
                }
            }
        }
    }
}

