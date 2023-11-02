using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using eBay.Service.Call;
using eBay.Service.Core.Sdk;
using eBay.Service.Core.Soap;
using System.Configuration;

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
        public class OauthViewModal
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public int refresh_token_expires_in { get; set; }
            public string token_type { get; set; }
            public static OauthViewModal operator +(OauthViewModal c1, OauthViewModal c2)
            {
                OauthViewModal temp = new OauthViewModal();
                temp.expires_in = c1.expires_in + c2.expires_in;
                //temp.y = c1.y + c2.y;
                return temp;
            }
        }
        public static string GetRefreshCode(string code)
        {
            var client = new HttpClient();

            var scopeList = /*ConfigurationManager.AppSettings["EbayAPI"]*/"https://api.ebay.com" + "/oauth/api_scope/sell.account https://api.ebay.com/oauth/api_scope/sell.inventory";

            var grant_type = new KeyValuePair<string, string>("grant_type", "refresh_token");
            var codeID = new KeyValuePair<string, string>("refresh_token", code);
            var redirect_uri = new KeyValuePair<string, string>("scope", scopeList);

            var data = new List<KeyValuePair<string, string>>();
            data.Add(grant_type);
            data.Add(codeID);
            data.Add(redirect_uri);
            var webRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.ebay.com/identity/v1/oauth2/token")
            {
                Content = new FormUrlEncodedContent(data)
            };

            var username = "Your UserName";// ConfigurationManager.AppSettings["AppID"];
            var password = "Your Password";// ConfigurationManager.AppSettings["CertID"];
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);

            var encodedstring = System.Convert.ToBase64String(plainTextBytes);


            client.DefaultRequestHeaders.Add("Authorization", "Basic " + encodedstring);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

            var response = client.SendAsync(webRequest).Result;

            var responsedata = response.Content.ReadAsStringAsync().Result;

            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<OauthViewModal>(responsedata);

            return obj.access_token;

        }
        public static StringCollection GetAllOrderIds()
        {
            Console.WriteLine("Orders Ids");
            
            string refreshToken = GetRefreshCode("Your Code");
            GetOrdersCall getOrdersApiCall = new GetOrdersCall(context);

            //time filter to get last 20 days orders
            TimeFilter timeFilter =new TimeFilter();
            timeFilter.TimeFrom = DateTime.UtcNow.AddDays(-90);
            timeFilter.TimeTo = DateTime.UtcNow;
            
            context.ApiCredential.eBayToken = refreshToken.Trim();

            DetailLevelCodeType[] detailLevels = new DetailLevelCodeType[] { DetailLevelCodeType.ReturnSummary };
            getOrdersApiCall.DetailLevelList = new DetailLevelCodeTypeCollection(detailLevels);
            //getOrdersApiCall.GetOrders(timeFilter, TradingRoleCodeType.Seller, OrderStatusCodeType.Completed); //This was originally in code I changed status to get all products
            getOrdersApiCall.GetOrders(timeFilter, TradingRoleCodeType.Seller, OrderStatusCodeType.Completed);

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
                timeFilter.TimeFrom = DateTime.UtcNow.AddDays(-90);
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
