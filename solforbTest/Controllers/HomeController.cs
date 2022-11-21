using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using solforbTest.Data.Entity;
using solforbTest.Data.Repository;
using solforbTest.Data.Repository.Implementation;
using solforbTest.Models;

namespace solforbTest.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IOrderRepository _orderRepository;

    private readonly IProviderRepository _providerRepository;

    private readonly IOrderItemRepository _orderItemRepository;

    public HomeController(
        ILogger<HomeController> logger,
        IOrderRepository orderRepository,
        IProviderRepository providerRepository,
        IOrderItemRepository orderItemRepository
        )
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _providerRepository = providerRepository;
        _orderItemRepository = orderItemRepository;
    }

    public IActionResult Index()
    {
        var orderDateFrom = DateTime.Now.AddMonths(-1);

        var orderDateTo = DateTime.Now;

        var orders = _orderRepository.GetAll().Result.Where(x => x.Date >= orderDateFrom && x.Date <= orderDateTo).ToList();
        var orderViewModels = new List<OrderViewModel>();

        orders.ForEach(x => orderViewModels.Add(new OrderViewModel(x)));

        var orderNumbers = new SelectList(orders.Select(x => x.Number).Distinct().ToList());
        var orderProviders = new SelectList(orders.Select(x => x.Provider.Name).Distinct().ToList());
        var orderItemNames = new SelectList(orders.SelectMany(x => x.OrderItems).Select(x => x.Name).Distinct().ToList());
        var orderItemUnits = new SelectList(orders.SelectMany(x => x.OrderItems).Select(x => x.Unit).Distinct().ToList());



        return View(
            new OrdersIndexViewModel()
            {
                OrderDateFrom = orderDateFrom,
                OrderDateTo = orderDateTo,
                OrderNumbers = orderNumbers,
                OrderProviders = orderProviders,
                OrderViewModels = orderViewModels,
                OrderItemNames = orderItemNames,
                OrderItemUnits = orderItemUnits
            });
    }


    [HttpPost]
    public IActionResult Index(OrdersIndexViewModel model)
    {
        var orderDateFrom = model.OrderDateFrom;

        var orderDateTo = model.OrderDateTo;

        var orders = _orderRepository
            .GetAll()
            .Result;

        var filtredOrders = orders.Where(x => x.Date >= orderDateFrom && x.Date <= orderDateTo);

        if(model.SelectedOrderNumbers != null && model.SelectedOrderNumbers.Count() > 0)
            filtredOrders = filtredOrders.Where(x => model.SelectedOrderNumbers.Contains(x.Number));

        if (model.SelectedOrderProviders != null && model.SelectedOrderProviders.Count() > 0)
            filtredOrders = filtredOrders.Where(x => model.SelectedOrderProviders.Contains(x.Provider.Name));

        if (model.SelectedOrderItemNames != null && model.SelectedOrderItemNames.Count() > 0)
            filtredOrders = filtredOrders.Where(x => x.OrderItems.Any(y => model.SelectedOrderItemNames.Contains(y.Name)));

        if (model.SelectedOrderItemUnits != null && model.SelectedOrderItemUnits.Count() > 0)
            filtredOrders = filtredOrders.Where(x => x.OrderItems.Any(y => model.SelectedOrderItemUnits.Contains(y.Unit)));


        var ordersToShow = filtredOrders.ToList();

        var orderViewModels = new List<OrderViewModel>();

        ordersToShow.ForEach(x => orderViewModels.Add(new OrderViewModel(x)));

        model.OrderViewModels = orderViewModels;
        model.OrderNumbers = new SelectList(orders.Select(x => x.Number).Distinct().ToList());
        model.OrderProviders = new SelectList(orders.Select(x => x.Provider.Name).Distinct().ToList());
        model.OrderItemNames = new SelectList(orders.SelectMany(x => x.OrderItems).Select(x => x.Name).Distinct().ToList());
        model.OrderItemUnits = new SelectList(orders.SelectMany(x => x.OrderItems).Select(x => x.Unit).Distinct().ToList());

        return View(model);

    }

    [HttpGet]
    public IActionResult UpsertNewOrder(int? id)
    {
        var providers = _providerRepository.GetAll().Result;

        if(!id.HasValue)
        {
            return View(
                new UpsertOrderViewModel()
                {
                    OrderItemViewModels = new List<OrderItemViewModel>(),
                    Providers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(providers.Select(x => x.Name).ToList())
                });
        }

        var order = _orderRepository.GetById(id).Result;
        var orderViewModels = new List<OrderItemViewModel>();
        order.OrderItems.ForEach(x => orderViewModels.Add(new OrderItemViewModel(x)));

        return View(new UpsertOrderViewModel()
        {
            Id = id,
            OrderDateOfCreate = order.Date,
            OrderItemViewModels = orderViewModels,
            OrderNumber = order.Number,
            Providers = new SelectList(providers.Select(x => x.Name).ToList()),
            ProviderName = order.Provider.Name
        });
    } 

   
    [HttpPost]
    public IActionResult UpsertNewOrder(UpsertOrderViewModel upsertOrderViewModel)
    {
        var provider = _providerRepository.GetByName(upsertOrderViewModel.ProviderName).Result;
        upsertOrderViewModel.Providers = new SelectList(_providerRepository.GetAll().Result.Select(x => x.Name).ToList());

        var order = new Order()
        {
            Number = upsertOrderViewModel.OrderNumber,
            Date = upsertOrderViewModel.OrderDateOfCreate,
            Provider = provider,
            ProviderId = provider.Id

        };

        if (upsertOrderViewModel.OrderItemViewModels != null && upsertOrderViewModel.OrderItemViewModels.Any(x => x.ItemName == upsertOrderViewModel.OrderNumber))
            ModelState.AddModelError("OrderItemViewModal[i].Name", "Название товара не может быть совпадать с номером заказа");
        var orderValidation = _orderRepository.GetByNumberAndProvider(upsertOrderViewModel.OrderNumber, provider.Id).Result;

                
        if (upsertOrderViewModel.Id.HasValue)
        {
            if (orderValidation != null && orderValidation.Id != upsertOrderViewModel.Id)
            {
                ModelState.AddModelError("OrderItemViewModal.OrderNumber", "Заказ с таким номером у такого провайдера уже существует");
            }

            if (!ModelState.IsValid)
                return View(upsertOrderViewModel);

            UpdateOrder(upsertOrderViewModel, order);
            return RedirectToAction("Index", "Home"); 
        }

        if (orderValidation != null)
            ModelState.AddModelError("OrderItemViewModal.OrderNumber", "Заказ с таким номером у такого провайдера уже существует");

        if (!ModelState.IsValid)
            return View(upsertOrderViewModel);


        InsertOrder(upsertOrderViewModel, order);

        return RedirectToAction("Index", "Home"); 
    }

    [HttpPost]
    public IActionResult UpsertNewOrderWithItems(UpsertOrderViewModel upsertOrderViewModel)
    {
        var providers = _providerRepository.GetAll().Result;
        if (upsertOrderViewModel.OrderItemViewModels == null)
            upsertOrderViewModel.OrderItemViewModels = new List<OrderItemViewModel>();

        upsertOrderViewModel.OrderItemViewModels.Add(new OrderItemViewModel() { ItemName = "", Quantity = 0, Unit = "" });

        upsertOrderViewModel.Providers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(providers.Select(x => x.Name).ToList());

        return View(upsertOrderViewModel);
    }

    public IActionResult UpsertNewOrderWithoutDeletedItems(UpsertOrderViewModel upsertOrderViewModel)
    {
        var providers = _providerRepository.GetAll().Result;
        if (upsertOrderViewModel.OrderItemViewModels == null)
            upsertOrderViewModel.OrderItemViewModels = new List<OrderItemViewModel>();

        upsertOrderViewModel.OrderItemViewModels.RemoveAll(x => x.IsRemoved);

        upsertOrderViewModel.Providers = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(providers.Select(x => x.Name).ToList());

        return View("UpsertNewOrderWithItems", upsertOrderViewModel);
    }

    private void InsertOrder(UpsertOrderViewModel upsertOrderViewModel, Order order)
    {
        _orderRepository.Add(order);

        if (upsertOrderViewModel.OrderItemViewModels != null)
        {
            var orderId = _orderRepository.GetByNumberAndProvider(order.Number, order.ProviderId).Result.Id;

            var orderItems = new List<OrderItem>();

            upsertOrderViewModel.OrderItemViewModels
                .ForEach(x => _orderItemRepository
                .Add(new OrderItem()
                {
                    Name = x.ItemName,
                    Quantity = x.Quantity,
                    Unit = x.Unit,
                    OrderId = orderId

                }));

        }
    }

    private void UpdateOrder(UpsertOrderViewModel upsertOrderViewModel, Order order)
    {
        order.Id = upsertOrderViewModel.Id.Value;


        if (upsertOrderViewModel.OrderItemViewModels != null)
        {
            var existedOrdrerItems = upsertOrderViewModel.OrderItemViewModels.Where(x => x.Id.HasValue).ToList();

            var notExistedOrderItems = upsertOrderViewModel.OrderItemViewModels.Where(x => !x.Id.HasValue).ToList();

            var orderItems = _orderItemRepository
                .GetByOrderId(order.Id)
                .Result;

            var itemsForDelete = orderItems
                .Where(x => upsertOrderViewModel.OrderItemViewModels
                .All(z => z.Id != x.Id))
                .ToList();

            notExistedOrderItems
            .ForEach(x => _orderItemRepository
            .Add(new OrderItem()
            {
                Name = x.ItemName,
                Quantity = x.Quantity,
                Unit = x.Unit,
                OrderId = order.Id
            }));

            existedOrdrerItems.ForEach(x => UpdateOrderFromViewModel(x, orderItems.Where(y => y.Id == x.Id).First()));


            if (itemsForDelete != null)
                itemsForDelete.ForEach(x => _orderItemRepository.Remove(x));
        }
        _orderRepository.Update(order);

    }

    private OrderItem UpdateOrderFromViewModel(OrderItemViewModel model, OrderItem orderItem)
    {
        orderItem.Name = model.ItemName;
        orderItem.Quantity = model.Quantity;
        orderItem.Unit = model.Unit;

        _orderItemRepository.Update(orderItem);
        return orderItem;
    }
}

