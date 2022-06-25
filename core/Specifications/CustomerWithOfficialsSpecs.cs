using System;
using System.Linq;
using System.Linq.Expressions;
using core.Entities;

namespace core.Specifications
{
     public class CustomerWithOfficialsSpecs : BaseSpecification<Customer>
     {
          public CustomerWithOfficialsSpecs(CustomerSpecParams custParams)
            : base(x => 
                (string.IsNullOrEmpty(custParams.CustomerCityName) || 
                  x.City.ToLower().Contains(custParams.CustomerCityName.ToLower())) &&
                (!custParams.CustomerIndustryId.HasValue || 
                  x.CustomerIndustries.Select(x => x.IndustryId).Contains(Convert.ToInt32(custParams.CustomerIndustryId))) &&
                (string.IsNullOrEmpty(custParams.CustomerType) || 
                  x.CustomerType.ToLower().Contains(custParams.CustomerType.ToLower())) &&
                (string.IsNullOrEmpty(custParams.Search) || 
                  x.CustomerName.ToLower().Contains(custParams.Search.ToLower())) &&
                (!custParams.IndustryId.HasValue || 
                  x.CustomerIndustries.Select(x => x.IndustryId).Contains(Convert.ToInt32(custParams.IndustryId)))
            )
          {
              if (custParams.IncludeOfficials==true ) AddInclude(x => x.CustomerOfficials);
              if (custParams.IncludeIndustries==true) AddInclude(x => x.CustomerIndustries);
              if (custParams.Sort=="name") AddOrderBy(x => x.CustomerName);
              if (custParams.Sort=="city") AddOrderBy(x => x.City);

              ApplyPaging(custParams.PageSize * (custParams.PageIndex - 1), custParams.PageSize);

              if (!string.IsNullOrEmpty(custParams.Sort)) {
                switch(custParams.Sort.ToLower()) {
                  case "name":
                    AddOrderBy(x => x.CustomerName);
                    break;
                  case "namedesc":
                    AddOrderByDescending(x => x.CustomerName);
                    break;
                  
                  case "typeasc":
                    AddOrderBy(x => x.CustomerType);
                    break;
                  
                  case "typedesc":
                    AddOrderByDescending(x => x.CustomerType);
                    break;
                  
                  case "city":
                    AddOrderBy(x => x.City);
                    break;
                  
                  case "citydesc":
                    AddOrderByDescending(x => x.City);
                    break;

                  default: AddOrderBy(x => x.CustomerName);
                    break;
                }
              }
          }

          public CustomerWithOfficialsSpecs(int id) 
            : base(x => x.Id == id)
          {
              AddInclude(x => x.CustomerOfficials);
              AddInclude(x => x.CustomerIndustries);
          }
     }
}