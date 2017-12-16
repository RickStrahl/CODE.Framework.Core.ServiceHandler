using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;

namespace Sample.Services.Implementation
{
    public class CustomerService : ICustomerService
    {

        public GetCustomersResponse GetCustomers()
        {
            return new GetCustomersResponse()
            {
                CustomerList = new List<Customer>() {
                    new Customer {
                         Name = "Rick Strahl",
                        Company = "West wind"
                    },
                    new Customer {
                         Name = "Markus Egger",
                        Company = "Eps Software"
                    },
                }
            };
        }

        public GetCustomerResponse GetCustomer(GetCustomerRequest request)
        {
            return new GetCustomerResponse()
            {
                Customer = new Customer() {
                    Id = request.Id,
                    Name = "Rick Strahl",
                    Company = "West wind"
                }                
            };
        }

    }

}
