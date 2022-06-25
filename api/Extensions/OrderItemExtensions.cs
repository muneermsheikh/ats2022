using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class OrderItemExtensions
    {
         public static async Task<int> OrderItemIdFromCategoryRef(this ATSContext context, string categoryref)
        {
            if(string.IsNullOrEmpty(categoryref)) return -1;
            
            int i = categoryref.IndexOf("-");
            if (i== -1) return -1;
            var orderno = categoryref.Substring(0,i);
            var srno = categoryref.Substring(i+1);
            if (string.IsNullOrEmpty(orderno)) return -1;
            if (string.IsNullOrEmpty(srno)) return -1;
            int iorderno = Convert.ToInt32(orderno);
            int isrno = Convert.ToInt32(srno);

            var qry = await (from o in context.Orders where o.OrderNo == iorderno 
                join item in context.OrderItems on o.Id equals item.OrderId 
                select item.Id).FirstOrDefaultAsync();
            return qry;

        }
    }
}