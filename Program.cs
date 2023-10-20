using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using eBay.Service.Call;
using eBay.Service.Core.Sdk;
using eBay.Service.Core.Soap;


namespace EbayServiceTesting
{
    public class Program
    {
        public static ApiContext context = new ApiContext();
        static void Main(string[] args)
        {
            var orderIds=GetAllOrderIds();
            foreach (var item in orderIds)
            {
                Console.WriteLine(item);
            }
            GetAllOrders(orderIds);
            Console.WriteLine();
        }
        public static StringCollection GetAllOrderIds()
        {
            Console.WriteLine("Orders Ids");
            //token get from database
            string refreshToken = "v^1.1#i^1#r^0#I^3#p^3#f^0#t^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";

            GetOrdersCall getOrdersApiCall = new GetOrdersCall(context);

            //time filter to get last 20 days orders
            TimeFilter timeFilter =new TimeFilter();
            timeFilter.TimeFrom = DateTime.UtcNow.AddDays(-20);
            timeFilter.TimeTo = DateTime.UtcNow;
            
            context.ApiCredential.eBayToken = refreshToken.Trim();

            DetailLevelCodeType[] detailLevels = new DetailLevelCodeType[] { DetailLevelCodeType.ReturnSummary };
            getOrdersApiCall.DetailLevelList = new DetailLevelCodeTypeCollection(detailLevels);
            //getOrdersApiCall.GetOrders(timeFilter, TradingRoleCodeType.Seller, OrderStatusCodeType.Completed); //This was originally in code I changed status to get all products
            getOrdersApiCall.GetOrders(timeFilter, TradingRoleCodeType.Seller, OrderStatusCodeType.All);

            PaginationType pagination = new PaginationType()
            {
                EntriesPerPage = 10,
                PageNumber = 0,
            };

            bool hasMoreOrders = false;

            StringCollection orderIds = new StringCollection();
            do
            {
                pagination.PageNumber += 1;

                getOrdersApiCall.Pagination = pagination;

                getOrdersApiCall.Execute();

                if (getOrdersApiCall.ApiResponse.Ack != AckCodeType.Failure)
                {

                    foreach (OrderType order in getOrdersApiCall.ApiResponse.OrderArray)
                    {
                        orderIds.Add(order.OrderID);
                    }

                    hasMoreOrders = getOrdersApiCall.HasMoreOrders;
                }
            } while (hasMoreOrders);
            return orderIds;
        }
        public static void GetAllOrders(StringCollection orderIds)
        {
            Console.WriteLine("\n");
            Console.WriteLine("Orders");

            GetOrdersCall getOrdersApiCall = new GetOrdersCall(context);
            getOrdersApiCall.IncludeFinalValueFee = true;
            DetailLevelCodeType[] detailLevels = new DetailLevelCodeType[] { DetailLevelCodeType.ReturnAll };
            getOrdersApiCall.DetailLevelList = new DetailLevelCodeTypeCollection(detailLevels);
            if (orderIds != null)
                getOrdersApiCall.OrderIDList = orderIds;

            try
            {
                //time filter to get last 20 days orders
                TimeFilter timeFilter = new TimeFilter();
                timeFilter.TimeFrom = DateTime.UtcNow.AddDays(-20);
                timeFilter.TimeTo = DateTime.UtcNow;

                //OrderTypeCollection orders = getOrdersApiCall.GetOrders(timeFilter, TradingRoleCodeType.Seller, OrderStatusCodeType.Completed); //This was originally in code I changed status to get all products
                OrderTypeCollection orders = getOrdersApiCall.GetOrders(timeFilter, TradingRoleCodeType.Seller, OrderStatusCodeType.All); ;

                foreach (OrderType order in orders)
                {

                    bool completed = order.OrderStatus == OrderStatusCodeType.Completed;
                    bool cancelled = order.OrderStatus == OrderStatusCodeType.Cancelled;
                    bool inactive = order.OrderStatus == OrderStatusCodeType.Inactive;
                    bool inprocess = order.OrderStatus == OrderStatusCodeType.Active;




                    foreach (eBay.Service.Core.Soap.TransactionType trans in order.TransactionArray)
                    {
                        #region Process each ebay transaction
                        // Check if this transaction has already be recorded in system.

                        String transId = trans.TransactionID;
                        if (transId == null || transId == "")
                        {

                            continue;
                        }
                        Console.WriteLine("BuyerUserID: "+order.BuyerUserID);
                        Console.WriteLine("OrderID: "+order.OrderID);
                        Console.WriteLine("TransactionID: "+trans.TransactionID);
                        Console.WriteLine("ShippedTimeSpecified: " + order.ShippedTimeSpecified);
                        Console.WriteLine("\n");
                        // 
                        if (order.PaidTimeSpecified == true && order.ShippedTimeSpecified == false)
                        {
                            //logic to save order data and return list;

                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
