using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartInventory
{
    public class InventoryMonitor
    {
        private InventoryManager inv;
        private bool running = true;

        public InventoryMonitor(InventoryManager inventory)
        {
            inv = inventory;
        }

        public void Start()
        {
            new Thread(() =>
            {
                while (running)
                {
                    foreach (var p in inv.GetProducts())
                    {
                        if (p.IsLowStock())
                            inv.RaiseLowStockAlert(p);
                    }
                    Thread.Sleep(10000); // check every 10s
                }
            }).Start();
        }

        public void Stop() => running = false;

        // Replace the OnLowStockAlert method with the following implementation
        private void OnLowStockAlert(Product p)
        {
            // Pass the product to RaiseLowStockAlert as required by its signature
            inv.RaiseLowStockAlert(p);
        }
    }
}
