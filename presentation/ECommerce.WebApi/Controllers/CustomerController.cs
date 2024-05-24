using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities.Concretes;
using ECommerce.Domain.ViewModels;
using ECommerce.Domain.ViewModels._Customer;
using ECommerce.Domain.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly IReadCustomerRepository _readCustomerRepo;
    private readonly IWriteCustomerRepository _writecustomerRepos;
    
    public CustomerController(IReadCustomerRepository readCustomerRepo, IWriteCustomerRepository writecustomerRepos)
    {
        _readCustomerRepo = readCustomerRepo;
        _writecustomerRepos = writecustomerRepos;
    }
    
    // -CRUD Operations

    [HttpPost("AddCustomer")]
    public async Task<IActionResult> Add(AddCustomerVM customerVm)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var customer = new Customer()
        {
            FirstName = customerVm.FirstName,
            LastName = customerVm.LastName,
            Address = customerVm.Address,
            Email = customerVm.Email,
            Password = customerVm.Password
        };

        await _writecustomerRepos.AddAsync(customer);
        await _writecustomerRepos.SaveChangeAsync();

        return Ok();
    }

    [HttpGet("GetAllCustomers")]
    public async Task<IActionResult> GetAll([FromQuery] PaginationVM paginationVM)
    {
        var customers = await _readCustomerRepo.GetAllAsync();
        var customerForPage = customers.ToList().
            Skip(paginationVM.Page*paginationVM.PageSize).Take(paginationVM.PageSize).ToList();

        var allCustomerVm = customers.Select(c => new GetCustomerVM()
        {
            FirstName = c.FirstName,
            LastName = c.LastName,
            Address = c.Address,
            Email = c.Email
        });

        return Ok(allCustomerVm);
    }

    [HttpPut("UpdateCustomer")]
    public async Task<IActionResult> Update(int id,AddCustomerVM customerVm)
    {
        var customer = await _readCustomerRepo.GetByIdAsync(id);

        if (customer == null)
            return NotFound();

        customer.FirstName = customerVm.FirstName;
        customer.LastName = customerVm.LastName;
        customer.Address = customerVm.Address;
        customer.Email = customerVm.Email;
        customer.Password = customerVm.Password;

        await _writecustomerRepos.UpdateAsync(customer);
        await _writecustomerRepos.SaveChangeAsync();

        return Ok();
    }

    [HttpDelete("DeleteCustomer")]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _readCustomerRepo.GetByIdAsync(id);

        if (customer == null)
            return NotFound();

        await _writecustomerRepos.DeleteAsync(customer.Id);
        await _writecustomerRepos.SaveChangeAsync();

        return Ok();
    }
    
    // -Additional Functionalities
    [HttpGet("GetOrdersOfCustomer")]
    public async Task<IActionResult> GetOrders(int customerId)
    {
        var orders = await _readCustomerRepo.GetOrdersOfCustomer(customerId);

        if (orders == null)
            return NotFound();

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
}
