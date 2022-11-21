using System;
using solforbTest.Data.Entity;

namespace solforbTest.Models
{
    public class OrderItemViewModel
    {
        public int? Id { get; set; }

        public string ItemName { get; set; }

        public decimal Quantity { get; set; }

        public string Unit { get; set; }

        public bool IsRemoved { get; set; }

        public OrderItemViewModel(OrderItem orderItem)
        {
            Id = orderItem.Id;
            ItemName = orderItem.Name;
            Unit = orderItem.Unit;
            Quantity = orderItem.Quantity;
        }

       

        public OrderItemViewModel()
        {

        }
    }
}

