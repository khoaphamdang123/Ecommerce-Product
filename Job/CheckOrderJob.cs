using System;
using Ecommerce_Product.Models;
using Ecommerce_Product.Repository;
using Quartz;
namespace Ecommerce_Product.Job;

public class CheckOrderJob : IJob
{
    private readonly IOrderRepository _order;

    public CheckOrderJob(IOrderRepository order)
    {
        _order = order;
    }

    public async Task Execute(IJobExecutionContext context)
    {   
        await this._order.checkOrderStatus();
        // var orders = await _order.GetOrders();
        // foreach (var order in orders)
        // {
        //     if (order.Status == 0)
        //     {
        //         order.Status = 1;
        //         await _order.UpdateOrder(order);
        //     }
        // }
    }

}
