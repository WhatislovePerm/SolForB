using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace solforbTest.Models
{
    public class OrdersIndexViewModel
    {
        public List<OrderViewModel> OrderViewModels { get; set; }

        public IEnumerable<string> SelectedOrderNumbers { get; set; }

        public IEnumerable<string> SelectedOrderProviders { get; set; }

        public IEnumerable<string> SelectedOrderItemNames { get; set; }

        public IEnumerable<string> SelectedOrderItemUnits { get; set; }

        public DateTime OrderDateFrom { get; set; }

        public DateTime OrderDateTo { get; set; }
 
        public SelectList OrderNumbers { get; set; }

        public SelectList OrderProviders { get; set; }

        public SelectList OrderItemNames { get; set; }

        public SelectList OrderItemUnits { get; set; }

    }
}

