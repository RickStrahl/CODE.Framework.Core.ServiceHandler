﻿using System;
using System.Runtime.Serialization;

namespace CODE.Framework.Core.ServiceHandler
{
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


        [DataMember]
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

    [DataContract]
    public class ErrorResponse : BaseServiceResponse
    {
        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string ExceptionType { get; set;  }

        public ErrorResponse()
        {
            Success = false;
        }

        public ErrorResponse(Exception ex)
        {
            ex = ex.GetBaseException();
            Success = false;
            FailureInformation =ex.Message;
#if DEBUG
            StackTrace = ex.StackTrace;
            Source = ex.Source;
            ExceptionType = ex.GetType().ToString();
#endif
            
        }

        public ErrorResponse(string errorMessage)
        {
            Success = false;
            FailureInformation = errorMessage;            
        }
    }
}
