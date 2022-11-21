using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace solforbTest.Models
{
    public class UpsertOrderViewModel
    {
        public int? Id { get; set; }

        [Display(Name="Номер")]
        public string OrderNumber { get; set; }

        [Display(Name = "Провайдер")]
        public string ProviderName { get; set; }

        [Display(Name = "Дата заказа")]
        public DateTime OrderDateOfCreate { get; set; }

        [Display(Name = "Товары")]
        public List<OrderItemViewModel>? OrderItemViewModels { get; set; }

        public SelectList? Providers { get; set; }
        
    }
}

