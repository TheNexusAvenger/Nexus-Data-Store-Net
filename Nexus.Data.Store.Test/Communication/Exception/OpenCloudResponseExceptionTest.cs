using System.Collections.Generic;
using System.Net;
using Nexus.Data.Store.Communication.Exception;
using Nexus.Data.Store.Communication.Response;
using NUnit.Framework;

namespace Nexus.Data.Store.Test.Communication.Exception
{
    public class OpenCloudResponseExceptionTest
    {
        /// <summary>
        /// Tests GetException with a bad API key.
        /// </summary>
        [Test]
        public void TestGetExceptionApiKey()
        {
            Assert.That(OpenCloudResponseException.GetException(new HttpResponse<ErrorResponse>()
            {
                Status = HttpStatusCode.Unauthorized,
                Body = null,
            }).GetType() == typeof(OpenCloudUnauthorizedException));
        }
        
        
        /// <summary>
        /// Tests GetException with a bad scope (incorrect game id, missing permission).
        /// </summary>
        [Test]
        public void TestGetExceptionBadScope()
        {
            Assert.That(OpenCloudResponseException.GetException(new HttpResponse<ErrorResponse>()
            {
                Status = HttpStatusCode.Forbidden,
                Body = new ErrorResponse()
                {
                    Error = "INSUFFICIENT_SCOPE",
                    Message = "The api key does not have sufficient scope to perform this operation.",
                    ErrorDetails = new List<ErrorResponseDetail>()
                    {
                        new ErrorResponseDetail()
                        {
                            ErrorDetailType = "DatastoreErrorInfo",
                            DatastoreErrorCode = "Forbidden",
                        }  
                    },
                }
            }).GetType() == typeof(OpenCloudInsufficientScopeException));
        }
        
        /// <summary>
        /// Tests GetException with a DataStore not found.
        /// </summary>
        [Test]
        public void TestGetExceptionDatastoreNotFound()
        {
            Assert.That(OpenCloudResponseException.GetException(new HttpResponse<ErrorResponse>()
            {
                Status = HttpStatusCode.NotFound,
                Body = new ErrorResponse()
                {
                    Error = "NOT_FOUND",
                    Message = "Datastore not found.",
                    ErrorDetails = new List<ErrorResponseDetail>()
                    {
                        new ErrorResponseDetail()
                        {
                            ErrorDetailType = "DatastoreErrorInfo",
                            DatastoreErrorCode = "DatastoreNotFound",
                        }  
                    },
                }
            }).GetType() == typeof(OpenCloudDataStoreNotFoundException));
        }
        
        /// <summary>
        /// Tests GetException with a DataStore entry not found.
        /// </summary>
        [Test]
        public void TestGetExceptionDatastoreEntryNotFound()
        {
            Assert.That(OpenCloudResponseException.GetException(new HttpResponse<ErrorResponse>()
            {
                Status = HttpStatusCode.NotFound,
                Body = new ErrorResponse()
                {
                    Error = "NOT_FOUND",
                    Message = "Entry not found in the datastore.",
                    ErrorDetails = new List<ErrorResponseDetail>()
                    {
                        new ErrorResponseDetail()
                        {
                            ErrorDetailType = "DatastoreErrorInfo",
                            DatastoreErrorCode = "EntryNotFound",
                        }  
                    },
                }
            }).GetType() == typeof(OpenCloudDataStoreEntryNotFoundException));
        }
    }
}