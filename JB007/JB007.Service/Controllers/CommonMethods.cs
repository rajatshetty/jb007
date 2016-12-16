using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace JB007.Service.Controllers
{
    public class CommonMethods : ApiController
    {
        public ApiMessageError error = new ApiMessageError() { message = "Model Validated No Error Found" };
        private static CommonMethods _CurrentObject;

        public static CommonMethods CurrentObject
        {
            get
            {
                if (_CurrentObject == null)
                    _CurrentObject = new CommonMethods();
                return _CurrentObject;
            }
        }

        public HttpResponseMessage IsModelStateValid(System.Web.Http.ModelBinding.ModelStateDictionary modelState, HttpRequestMessage request)
        {
            if (!modelState.IsValid)
            {
                // add errors into our client error model for client
                foreach (var prop in modelState.Values)
                {
                    var modelError = prop.Errors.FirstOrDefault();
                    if (!string.IsNullOrEmpty(modelError.ErrorMessage))
                        error.errors.Add(modelError.ErrorMessage);
                    else
                        error.errors.Add(modelError.Exception.Message);
                }
                return request.CreateResponse<ApiMessageError>(HttpStatusCode.Conflict, error);
            }
            else
                return request.CreateResponse<ApiMessageError>(HttpStatusCode.OK, error);
        }

    }
}
