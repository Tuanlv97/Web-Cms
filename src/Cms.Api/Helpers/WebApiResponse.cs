//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Web;

//namespace Cms.Api.Helpers
//{
//    public class WebApiResponse
//    {
//        public WebApiResponse(T apiResponse, HttpStatusCode httpStatusCode)
//        {
//            ApiResponse = apiResponse;
//            HttpStatusCode = httpStatusCode;
//        }

//        public WebApiResponse(string error, HttpStatusCode httpStatusCode, bool isError) // isError is just a way to differentiate the two constructors. If <code>T</code> were a string this constructor would always be called. 
//        {
//            Error = error;
//            HttpStatusCode = httpStatusCode;
//        }
//        public T ApiResponse { get; set; }
//        public HttpStatusCode HttpStatusCode { get; set; }
//        public string Error { get; set; }
//    }
//}