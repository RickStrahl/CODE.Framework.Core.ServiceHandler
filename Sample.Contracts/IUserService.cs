using CODE.Framework.Core.ServiceHandler;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sample.Contracts
{
    
    public interface IUserService
    {
        [Rest(Method = RestMethods.Post)]
        AuthenticateUserResponse AuthenticateUser(AuthenticateUserRequest request);

        [Rest(Method = RestMethods.Get)]
        SignoutResponse Signout(SignoutRequest request);

        [Rest(Method = RestMethods.Get)]
        IsAuthenticatedResponse IsAuthenticated(IsAuthenticatedRequest request);

        [Rest(Method = RestMethods.Get)]
        GetUserResponse GetUser(GetUserRequest request);

        [Rest(Method = RestMethods.Post)]
        SaveUserResponse SaveUser(SaveUserRequest request);

        [Rest(Method = RestMethods.Post)]
        ResetPasswordResponse ResetPassword(ResetPasswordRequest request);
    }


    [DataContract]
    public class BaseServiceResponse
    {
        public BaseServiceResponse()
        {
            Success = true;
        }

        /// <summary>
        /// Gets or sets the success.
        /// </summary>
        /// <value>The success.</value>
        [DataMember(IsRequired = true)]
        public bool Success { get; set; }


        [DataMember(IsRequired = true)]
        public string FailureInformation { get; set; }

        public void SetError(string message, Exception ex = null)
        {
            if (message != null)
            {
                Success = false;
                FailureInformation = message;
            }
            else
            {
                Success = true;
                FailureInformation = null;
            }
        }

    }

    public class GetUserRequest : BaseServiceRequest
    {
        [DataMember(IsRequired = true)]
        public string Id { get; set; }
    }

    public class GetUserResponse : BaseServiceResponse
    {
        public GetUserResponse()
        {
            Roles = new List<string>();
        }

        [DataMember(IsRequired = true)]
        public string UserId { get; set; }

        [DataMember(IsRequired = true)]
        public string Username { get; set; }

        [DataMember(IsRequired = true)]
        public string Email { get; set; }


        [DataMember(IsRequired = true)]
        public string Firstname { get; set; }

        [DataMember(IsRequired = true)]
        public string Lastname { get; set; }

        [DataMember(IsRequired = true)]
        public string Company { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public string Verifier { get; set; }

        [DataMember]
        public DateTime LastLogin { get; set; }

        [DataMember]
        public List<string> Roles { get; set; }

    }

    [DataContract]
    public class BaseServiceRequest
    {

    }

    public class SignoutRequest : BaseServiceRequest
    {

    }

    [DataContract]
    public class ResetPasswordRequest : BaseServiceRequest
    {
        [DataMember(IsRequired = true)]
        public string Username { get; set; }
    }

    [DataContract]
    public class ResetPasswordResponse : BaseServiceResponse
    {
    }


    public class SignoutResponse : BaseServiceResponse
    {
    }

    public class IsAuthenticatedRequest : BaseServiceRequest
    {

    }

    public class IsAuthenticatedResponse : BaseServiceResponse
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string UserId { get; set; }
    }


    public class AuthenticateUserRequest : BaseServiceRequest
    {
        public AuthenticateUserRequest()
        {
            UserName = string.Empty;
            //Password = string.Empty;
            RememberMe = false;
        }

        [DataMember(IsRequired = true)]
        public string UserName { get; set; }
        [DataMember(IsRequired = true)]
        public string Password { get; set; }
        [DataMember(IsRequired = true)]
        public bool RememberMe { get; set; }
    }

    [DataContract]
    public class AuthenticateUserResponse : BaseServiceResponse
    {
        public AuthenticateUserResponse()
        {
            Success = true;
            FailureInformation = string.Empty;
            Roles = new List<string>();
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Firstname { get; set; }

        [DataMember]
        public string Lastname { get; set; }

        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public List<string> Roles { get; set; }
    }

    [DataContract]
    public class SaveUserRequest
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember(IsRequired = true)]
        public string Username { get; set; }

        [DataMember]
        public string Email { get; set; }


        [DataMember(IsRequired = true)]
        public string Firstname { get; set; }

        [DataMember(IsRequired = true)]
        public string Lastname { get; set; }

        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public string Verifier { get; set; }

        [DataMember]
        public DateTime LastLogin { get; set; }

        [DataMember]
        public List<string> Roles { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string PasswordVerify { get; set; }
    }

    [DataContract]
    public class SaveUserResponse
    {
        /// <summary>
        /// Gets or sets the success status
        /// </summary>        
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// Error message if an error occurred and Success=false
        /// </summary>
        [DataMember]
        public string FailureInformation { get; set; }

        /// <summary>
        /// Id of the updated or new User
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }
    }
}
