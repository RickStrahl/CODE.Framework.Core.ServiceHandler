﻿using Microsoft.Extensions.Configuration;
using Sample.Contracts;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Services.Implementation
{
    public class CustomerService : ICustomerService
    {
        IConfiguration Config;

        /// <summary>
        /// You can optionally inject any DI dependencies
        /// </summary>
        /// <param name="config"></param>
        public CustomerService(IConfiguration config)
        {
            var Config = config;
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

        public async Task<GetCustomerResponse> GetCustomer(GetCustomerRequest request)
        {
            var result = new GetCustomerResponse()
            {
                Customer = new Customer() {
                    Id = request.Id,
                    Name = "Rick Strahl",
                    Company = "West wind"
                }                
            };

            return result;
        }

    }

}
