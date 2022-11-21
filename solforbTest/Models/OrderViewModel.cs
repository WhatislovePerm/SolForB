using System;
using solforbTest.Data.Entity;

namespace solforbTest.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public DateTime DateOfCreate { get; set; }

        public string ProviderName { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; }

        public OrderViewModel(Order order)
        {
            Id = order.Id;

            Number = order.Number;

            DateOfCreate = order.Date;

            ProviderName = order.Provider.Name;

            OrderItems = order.OrderItems.Select(x => new OrderItemViewModel(x)).ToList();
        }
        
    }
}

