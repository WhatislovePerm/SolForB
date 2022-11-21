using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using solforbTest.Data.Repository.Implementation;
using solforbTest.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace solforbTest.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;

        private readonly IOrderRepository _orderRepository;

        public OrderController(ILogger<OrderController> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public IActionResult Index(int id)
        {

            var order = _orderRepository.GetById(id).Result;
            var orderViewModels = new List<OrderItemViewModel>();
            order.OrderItems.ForEach(x => orderViewModels.Add(new OrderItemViewModel(x)));

            return View(new UpsertOrderViewModel()
            {
                Id = id,
                OrderDateOfCreate = order.Date,
                OrderItemViewModels = orderViewModels,
                OrderNumber = order.Number,
                ProviderName = order.Provider.Name
            });
        }

        public IActionResult Delete(int? id)
        {
            var order = _orderRepository.GetById(id).Result;
            _orderRepository.Remove(order);

            return RedirectToAction("Index", "Home");
        }
    }
}

