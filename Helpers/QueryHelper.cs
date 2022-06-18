using Fibonacci_one.Repository;
using Fibonacci_one.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fibonacci_one.Helpers
{
    internal class QueryHelper
    {
        NorthwndDbContext _context;

        public QueryHelper(NorthwndDbContext context)
        {
            _context = context;
        }

        //Londra da ikamet eden calisanlar
        public IQueryable<Employee> GetLondonEmployees()
        {
            return _context.Employees.Where(e => e.City == "London");
        }
        //Almanca bilen calisanlar
        public IQueryable<Employee> GetGermanLangEmployees()
        {
            return _context.Employees.Where(e => e.Notes.Contains("German"));
        }
        //Teslimati gerceklesmemis siparisler
        public IQueryable<Order> GetUnshippedOrders()
        {
            return _context.Orders.Where(o => o.ShippedDate == null);
        }
        //Birim fiyati 50 ile 80 arasinda olan urunler
        public IQueryable<Product> GetProductsInRange(int x, int y)
        {
            return _context.Products.Where(p => p.UnitPrice >= x && p.UnitPrice <= y);
        }
        //Sisede satilan urunler
        public IQueryable<Product> GetProductsByPacketType(string packetType)
        {
            return _context.Products.Where(p => p.QuantityPerUnit.Contains(packetType));
        }
        //Teslim edilen siparisler kac gunde teslim edildi
        public IQueryable<Order> GetOrdersShippedDateDetails()
        {
            var shippedOrder = _context.Orders.Where(o => o.ShippedDate != null);
            return shippedOrder;
        }
        //Stok miktarı kritik seviyede olan ürünler
        public IQueryable<Product> GetProductsByLowStock()
        {
            return _context.Products.Where(p => p.UnitsInStock < p.ReorderLevel);
        }
        //Seafood kategorisinden ürün alan müşteriler
        public IList<IQueryable<string>> GetCustomersByCategory(string category)
        {
            var categoryId = _context.Categories.Where(c => c.CategoryName == category)
            .Select(c => c.CategoryId);
            var orderIds = _context.OrderDetails.Where(od => od.ProductId.Equals(categoryId.FirstOrDefault())).Select(od => od.OrderId);
            var orders = _context.Orders.Where(o => orderIds.Contains(o.OrderId))
            .Select(o => o.CustomerId);
            IList<IQueryable<string>> customer = new List<IQueryable<string>>();
            foreach (var i in orders)
            {
                customer.Add(_context.Customers.Where(c => c.CustomerId.Equals(i)).Select(c => c.ContactName));
            }
            return customer;
        }
        //Hangi kargo şirketine ne kadar ödeme yapılmış?
        public Dictionary<IQueryable<string?>, IQueryable<decimal?>> GetShippingPaySum()
        {
            var shipperCount = _context.Shippers.Select(s => s.ShipperId);
            Dictionary<IQueryable<string?>, IQueryable<decimal?>> paySum = new Dictionary<IQueryable<string?>, IQueryable<decimal?>>();
            foreach (var i in shipperCount)
            {
                paySum.Add(_context.Shippers.Where(s => s.ShipperId == i).Select(s => s.CompanyName),
                    _context.Orders.Where(o => o.ShippedDate != null && o.ShipVia == i).Select(o => o.Freight));
            }
            return paySum;
        }
        //Her yıl hangi ülkeye kaç adet sipariş gönderilmiştir?
        public System.Collections.ArrayList GetOrdersByYear()
        {
            var list = new System.Collections.ArrayList();
            var allsc = _context.Orders.Where(o => o.ShipCountry != null).ToList();
            var sc = allsc.Select(c => c.ShipCountry).Distinct();
            var yy = allsc.Select(c => c.OrderDate.Value.Year).Distinct();
            foreach (var y in yy)
                foreach (var i in sc) list.Add($"{y} - {(i.Length <= 11 ? i.PadRight(11 - i.Length + i.Length) : i)}: {allsc.Where(o => o.ShipCountry == i && o.OrderDate.Value.Year == y).Count()}");
            return list;
        }
        //Dörtten az sipariş veren müşteriler hangileridir?
        public IQueryable<Customer> GetCustomerByOrderCount(int orderCount)
        {
            var customers = _context.Customers.Where(c => c.Orders.Count() < orderCount);
            return customers;
        }
        //New York şehrinden sorumlu olan çalışanlar
        public Employee GetEmployeesByCity(string city)
        {
            var territory = _context.Territories.Where(e => e.TerritoryDescription == city).Select(t => t.TerritoryId);
            var empTerritory = _context.Territories.Where(e => territory.Contains(e.TerritoryId)).Select(e => e.Employees);
            var emp = empTerritory.Select(e => e.FirstOrDefault()).FirstOrDefault();
            return emp;
        }
        // Satışlar kaç günde teslim edilmiş? SiparisNo | GunSayisi
        //GoTo GetOrdersShippedDateDetails
        //-----------------------------------//
        //Kargo ücreti, içerdiği en pahalı üründen fazla olan siparişler hangileridir?
        public void GetOrdersByMaxFreight()
        {
            Console.WriteLine("\nOrders by max freight and MostExpensiveProductPrice:");
            Console.WriteLine("==========================");
            var maxFreight = _context.OrderDetails.Where(o => o.UnitPrice == _context.OrderDetails.
            Where(od => od.OrderId == o.OrderId).Max(od => od.UnitPrice));
            var maxf = _context.Orders.Where(o => o.Freight >= maxFreight.
            Where(od => od.OrderId == o.OrderId).Max(od => od.UnitPrice));
            var fullData = maxf.Join(_context.Products,
               c => maxFreight.Where(od => od.OrderId == c.OrderId).Select(od => od.ProductId).FirstOrDefault(),
               o => o.ProductId,
               (c, o) => new
               {
                   OrderId = c.OrderId,
                   ProductName = o.ProductName,
                   Price = (float)maxFreight.Where(od => od.OrderId == c.OrderId).Select(od => od.UnitPrice).FirstOrDefault(),
                   ShipFee = c.Freight
               });
            foreach (var i in fullData) Console.WriteLine("{0} | {1} | ExpensiveProductPrice: {2:###.###} | ShippingFee: {3:###.##}",
                i.OrderId, (i.ProductName.Length <= 32 ? i.ProductName.PadRight(32 - i.ProductName.Length + i.ProductName.Length) : i.ProductName),
                i.Price.ToString().Length > 6 ? i.Price : i.Price.ToString().PadLeft(6 - i.Price.ToString().Length + i.Price.ToString().Length),
                i.ShipFee
            );
        }
        //Her yılın en çok taşıma işlemi yapan kargo firması hangisidir?
        public void GetShippersByMaxShipmentsByYear()
        {
            Console.WriteLine("\nShippers by max shipments by year:");
            Console.WriteLine("====================================");
            var maxShipments = _context.Orders.Where(o => o.ShippedDate != null)
                .Join(_context.Shippers,
                       c=>c.ShipVia,
                       s=>s.ShipperId,
                       (c,s)=>new {shippingCompanyName=s.CompanyName, OrderDate=c.OrderDate, ShipVia=c.ShipVia });
            var allsc = _context.Orders.Where(o => o.ShippedDate != null).ToList();
            var yy = allsc.Select(c => c.OrderDate.Value.Year).Distinct();
            foreach (var i in yy)
            {
                var bb = maxShipments.Where(o => o.OrderDate.Value.Year == i)
                        .GroupBy(o => o.shippingCompanyName).Select(g => new { i = g.Key, Count = g.Count() }).OrderByDescending(g => g.Count).FirstOrDefault();
                Console.WriteLine("Year: {0} | Company: {1} | Number of transports: {2}", i ,bb.i ,bb.Count);
            }
        }
    }
}
