using System;
using System.Collections.Generic;
using System.Text;

namespace CODE.Framework.Core.ServiceHandler
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RestAttribute : Attribute
    {
        public RestAttribute()
        {

        }

        /// <summary>
        /// HTTP method used
        /// </summary>
        public RestMethods Method { get; set; }

        public string Name { get; set; }
    }

    public enum RestMethods
    {
        //
        // Summary:
        //     HTTP POST
        Post = 0,
        //
        // Summary:
        //     HTTP GET
        Get = 1,
        //
        // Summary:
        //     HTTP PUT
        Put = 2,
        //
        // Summary:
        //     HTTP DELETE
        Delete = 3,
        //
        // Summary:
        //     HTTP HEAD
        Head = 4,
        //
        // Summary:
        //     HTTP TRACE
        Trace = 5,
        //
        // Summary:
        //     HTTP SEARCH
        Search = 6,
        //
        // Summary:
        //     HTTP CONNECT
        Connect = 7,
        //
        // Summary:
        //     HTTP PROPFIND
        PropFind = 8,
        //
        // Summary:
        //     HTTP PROPPATCH
        PropPatch = 9,
        //
        // Summary:
        //     HTTP PATCH
        Patch = 10,
        //
        // Summary:
        //     HTTP MKCOL
        Mkcol = 11,
        //
        // Summary:
        //     HTTP COPY
        Copy = 12,
        //
        // Summary:
        //     HTTP MOVE
        Move = 13,
        //
        // Summary:
        //     HTTP LOCK
        Lock = 14,
        //
        // Summary:
        //     HTTP UNLOCK
        Unlock = 15,
        //
        // Summary:
        //     HTTP OPTIONS
        Options = 16
    }
}
