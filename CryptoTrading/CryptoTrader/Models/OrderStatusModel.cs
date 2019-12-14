using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoTrader.Models
{
    public class OrderStatusModel
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<OrderStatusResultModel> result { get; set; }
    }

    public class OrderStatusModel2
    {
        public bool success { get; set; }
        public string message { get; set; }
        public OrderStatusResultModel result { get; set; }
    }
    public class OrderStatusResultModel
    {
        public string Uuid { get; set; }
        public string OrderUuid { get; set; }
        public string Exchange { get; set; }
        public string OrderType { get; set; }
        public double Quantity { get; set; }
        public double QuantityRemaining { get; set; }
        public double Limit { get; set; }
        public double CommissionPaid { get; set; }
        public double Price { get; set; }
        public object PricePerUnit { get; set; }
        public DateTime Opened { get; set; }
        public DateTime? Closed { get; set; }
        public string CancelInitiated { get; set; }
        public string ImmediateOrCancel { get; set; }
        public string IsConditional { get; set; }
    }
}
