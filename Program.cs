using Fibonacci_one.Helpers;
using Fibonacci_one.Repository;

namespace Fibonacci_one
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NorthwndDbContext context = new();
            QueryHelper queryHelper = new(context);
            var londraEmployees = queryHelper.GetLondonEmployees();
            Console.WriteLine("London employees:");
            Console.WriteLine("==================");
            foreach (var employee in londraEmployees)
            {
                Console.WriteLine(string.Concat(employee.FirstName, " ", employee.LastName));
            }
            var germanLangEmp = queryHelper.GetGermanLangEmployees();
            Console.WriteLine("\nGerman language employees:");
            Console.WriteLine("==========================");
            foreach (var employee in germanLangEmp)
            {
                Console.WriteLine(string.Concat(employee.EmployeeId + " | " + employee.FirstName, " ", employee.LastName));
            }
            var notYetShip = queryHelper.GetUnshippedOrders();
            Console.WriteLine("\nNot yet shipped orders:");
            Console.WriteLine("========================");
            foreach (var order in notYetShip)
            {
                Console.WriteLine(string.Concat(order.OrderId));
            }
            var inRangePrices = queryHelper.GetProductsInRange(50, 80);
            Console.WriteLine("\nProducts price in range 50-80:");
            Console.WriteLine("==============================");
            foreach (var product in inRangePrices)
            {
                Console.WriteLine(string.Concat(product.ProductName, " ", product.UnitPrice));
            }
            var bottleProd = queryHelper.GetProductsByPacketType("bottle");
            Console.WriteLine("\nProducts by packet type bottle:");
            Console.WriteLine("==============================");
            foreach (var product in bottleProd)
            {
                Console.WriteLine(string.Concat(product.ProductName, " ", product.UnitsInStock));
            }
            var deliveryDetail = queryHelper.GetOrdersShippedDateDetails();
            Console.WriteLine("\nOrders shipped date details:");
            Console.WriteLine("==============================");
            foreach (var order in deliveryDetail)
            {
                var orderDate = order.OrderDate.Value;
                var shippedDate = order.ShippedDate.Value;
                Console.WriteLine("{0} | {1} | {2} | {3}", order.OrderId, orderDate.ToShortDateString().Length < 10 ? " 0" + orderDate.ToShortDateString() : orderDate.ToShortDateString(), shippedDate.ToShortDateString().Length < 10 ? " 0" + shippedDate.ToShortDateString() : shippedDate.ToShortDateString(), (shippedDate - orderDate).Days);
            }
            var lowStock = queryHelper.GetProductsByLowStock();
            Console.WriteLine("\nProducts by low stock:");
            Console.WriteLine("=======================");
            foreach (var product in lowStock)
            {
                Console.WriteLine(string.Concat(product.ProductName, " ", product.UnitsInStock));
            }
            var seafoodCustomers = queryHelper.GetCustomersByCategory("Seafood");
            Console.WriteLine("\nCustomers by seafood category:");
            Console.WriteLine("==============================");
            foreach (var customer in seafoodCustomers)
            {
                Console.WriteLine(customer.FirstOrDefault());
            }
            var shippingPaySum = queryHelper.GetShippingPaySum();
            Console.WriteLine("\nShipping pay sum:");
            Console.WriteLine("==================");
            foreach (var i in shippingPaySum)
            {
                Console.WriteLine(string.Concat(i.Key.FirstOrDefault() + " : " + i.Value.FirstOrDefault() + " $"));
            }
            var everyYearCountOrders = queryHelper.GetOrdersByYear();
            Console.WriteLine("\nOrders by year:");
            Console.WriteLine("==================");
            Console.WriteLine("Year | Country | OrderCount");
            foreach (var i in everyYearCountOrders)
            {
                Console.WriteLine(i);
            }
            var cusByOrderC = queryHelper.GetCustomerByOrderCount(4);
            Console.WriteLine("\nCustomers by order count:");
            Console.WriteLine("==========================");
            foreach (var c in cusByOrderC)
            {
                Console.WriteLine($"{c.CustomerId} - {c.ContactName}");
            }
            var empByCity = queryHelper.GetEmployeesByCity("New York");
            Console.WriteLine("\nEmployees by city:");
            Console.WriteLine("==================");
            Console.WriteLine("Employee responsible for New York: {0}", empByCity.FirstName + ' ' + empByCity.LastName);

            Console.WriteLine("\nOrders delivered day details:");
            Console.WriteLine("==============================");
            foreach (var order in deliveryDetail)
            {
                var orderDate = order.OrderDate.Value;
                var shippedDate = order.ShippedDate.Value;
                Console.WriteLine("OrderId: {0} | delivered {1} at days ", order.OrderId, (shippedDate - orderDate).Days);
            }
            queryHelper.GetOrdersByMaxFreight();
            queryHelper.GetShippersByMaxShipmentsByYear();

        }
    }
}