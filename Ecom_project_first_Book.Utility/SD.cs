using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom_project_first_Book.Utility
{
    public static class SD
    {
        //Roles

        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
        public const string Role_Company = "Company";
        public const string Role_Individual = "Individiual";

        //SP Cover Type

        public const string SP_GetCoverTypes = "SP_GetCoverTypes";
        public const string SP_GetCoverType = "SP_GetCoverType";
        public const string SP_CreateCoverType = "SP_CreateCoverType";
        public const string SP_UpdateCoverType = "SP_UpdateCoverType";
        public const string SP_DeleteCoverType = "SP_DeleteCoverType";

        //Session Cart Count 

        public const string Ss_CartSessionCount = "Cart Count Session";

        public static double GetPriceBasedOnQuantity(double quantity, double price, double price50, double price100)
        {
            if (quantity < 50)
                return price;
            else if (quantity < 100)
                return price50;
            else return price100;
        }
        //Order Status
        public const string OrderStatusPending = "Pending";
        public const string OrderStatusApproved = "Approved";
        public const string OrderStatusInProgress = "Processing";
        public const string OrderStatusShipped = "Shipped";
        public const string OrderStatusCancelled = "Cancelled";
        public const string OrderStatusRefunded = "Refunded";

        //Payment Status
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayPayment = "PaymentStatusDelay";
        public const string PaymentStatusRejected = "Rejected";

    }
}
