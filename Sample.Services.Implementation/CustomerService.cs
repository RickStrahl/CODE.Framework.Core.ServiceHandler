using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;

namespace Sample.Services.Implementation
{
    public class CustomerService : ICustomerService
    {

        /// <summary>
        /// Simulate User Principal 
        /// </summary>
        public ClaimsPrincipal User
        {
            get
            {
                if (_user == null)
                    _user = Thread.CurrentPrincipal as ClaimsPrincipal;

                return _user;
            }
        }
        private ClaimsPrincipal _user = null;

     

        public CustomerService()
        {
            
        }

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
