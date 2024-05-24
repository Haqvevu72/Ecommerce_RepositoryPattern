using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities.Concretes;
using ECommerce.Domain.ViewModels;
using ECommerce.Domain.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IReadOrderRepository _readOrderRepo;
    private readonly IWriteOrderRepository _writeOrderRepo;
    private readonly IReadCustomerRepository _readCustomerRepo;
    
    public OrderController(IReadOrderRepository readOrderRepo, IWriteOrderRepository writeOrderRepo, IReadCustomerRepository readCustomerRepo)
    {
        _readOrderRepo = readOrderRepo;
        _writeOrderRepo = writeOrderRepo;
        _readCustomerRepo = readCustomerRepo;
    }

    [HttpPost("AddOrder")]
    public async Task<IActionResult> Add([FromBody]AddOrderVM addOrderVm)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        if (! await IsCustomerExist(addOrderVm.CustomerId))
            return NotFound("Customer not found");

        var order = new Order()
        {
            OrderNumber = addOrderVm.OrderNumber,
            OrderDate = addOrderVm.OrderDate,
            OrderNote = addOrderVm.OrderNote,
            Total = addOrderVm.Total,
            CustomerId = addOrderVm.CustomerId
        };

        await _writeOrderRepo.AddAsync(order);
        await _writeOrderRepo.SaveChangeAsync();

        return Ok();
    }

    [HttpGet("GetAllOrders")]
    public async Task<IActionResult> GetAll([FromQuery] PaginationVM paginationVM)
    {
        var orders = await _readOrderRepo.GetAllAsync();
        var orderForPage = orders.ToList()
            .Skip(paginationVM.Page * paginationVM.PageSize).Take(paginationVM.PageSize).ToList();

        var allOrdersVm = orders.Select(o => new GetOrderVM()
        {
            OrderNumber = o.OrderNumber,
            OrderDate = o.OrderDate,
            OrderNote = o.OrderNote,
            Total = o.Total,
            CustomerId = o.CustomerId
        });

        return Ok(allOrdersVm);

    }

    [HttpPut("UpdateOrder")]
    public async Task<IActionResult> Update(int id, AddOrderVM addOrderVm)
    {
        var order = await _readOrderRepo.GetByIdAsync(id);

        if (order == null)
            return NotFound("Order Not Found");

        if (!await IsCustomerExist(addOrderVm.CustomerId))
            return NotFound("Customer Not Found");

        order.OrderNumber = addOrderVm.OrderNumber;
        order.OrderDate = addOrderVm.OrderDate;
        order.OrderNote = addOrderVm.OrderNote;
        order.Total = addOrderVm.Total;
        order.CustomerId = addOrderVm.CustomerId;

        await _writeOrderRepo.UpdateAsync(order);
        await _writeOrderRepo.SaveChangeAsync();

        return Ok();
    }

    [HttpDelete("DeleteOrder")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _readOrderRepo.GetByIdAsync(id);
        if (order == null)
            return NotFound();
        await _writeOrderRepo.DeleteAsync(id);
        await _writeOrderRepo.SaveChangeAsync();

        return Ok();
    }

    private async Task<bool> IsCustomerExist(int id)
    {
        var result = await _readCustomerRepo.GetByIdAsync(id);
        if (result == null)
            return false;
        return true;
    }
}
