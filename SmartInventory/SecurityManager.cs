using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventory
{
    public class SecurityManager
    {
        
        private readonly Dictionary<string, (string Password, string Role)> users = new Dictionary<string, (string Password, string Role)>();

        public string CurrentUserRole { get; private set; }

        public SecurityManager()
        {
            // sample users
            users["admin"] = ("admin", "Admin");
            users["manager"] = ("manager", "Manager");
            users["emp"] = ("emp", "Employee");
        }

        public bool Authenticate(string username, string password)
        {
            if (users.ContainsKey(username) && users[username].Password == password)
            {
                CurrentUserRole = users[username].Role;
                return true;
            }
            return false;
        }

        public bool HasPermission(string action)
        {
            if (CurrentUserRole == "Admin") return true;
            if (CurrentUserRole == "Manager" && action != "Delete") return true;
            if (CurrentUserRole == "Employee" && (action == "View" || action == "Update")) return true;
            return false;
        }
    }
}
