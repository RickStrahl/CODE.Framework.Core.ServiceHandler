using CODE.Framework.Core.ServiceHandler;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sample.Contracts
{
    public interface ICustomerService
    {
        [Rest(Method = RestMethods.Get,Name="")]
        GetCustomersResponse GetCustomers();

        [Rest(Method = RestMethods.Post)]
        GetCustomerResponse GetCustomer(GetCustomerRequest request);
 
    }


    [DataContract]
    public class GetCustomerRequest : BaseServiceRequest
    {
        [DataMember]
        public string Id { get; set; }
    }

    [DataContract]
    public class GetCustomersResponse : BaseServiceResponse
    {
        [DataMember]
        public List<Customer> CustomerList { get; set; }
    }

    [DataContract]
    public class GetCustomerResponse : BaseServiceResponse
    {
        [DataMember]
        public Customer Customer { get; set; }
    }


    public class Customer
    {
        public string Name { get; set; }
        public string Company { get; set; }
        public string Id { get; set; }
    }

}
