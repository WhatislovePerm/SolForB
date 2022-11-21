using System;
namespace solforbTest.Data.Entity
{
    public class Order
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public DateTime Date { get; set; }

        public int ProviderId { get; set; }

        public Provider? Provider { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}

