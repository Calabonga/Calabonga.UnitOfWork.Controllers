using System;
using Calabonga.OperationResults;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Calabonga.UnitOfWork.Controllers.Controllers
{
    /// <summary>
    /// Base controller for PricePoint wrapped with OperationResult
    /// </summary>
    [ApiController]
    [EnableCors("CorsPolicy")]
    public abstract class OperationResultController : Controller
    {
        #region fields
        private readonly string _anonymousName;
        #endregion

        protected OperationResultController()
        {
            _anonymousName = "Anonymous";
        }


        /// <summary>
        /// OperationResult Response
        /// </summary>
        /// <param name="operationResult"></param>
        /// <returns></returns>
        [NonAction]
        protected ActionResult<OperationResult<TResult>> OperationResultResponse<TResult>(OperationResult<TResult> operationResult)
        {
            return Ok(operationResult);
        }

        /// <summary>
        /// OperationResult Response
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        protected ActionResult<OperationResult<TResult>> OperationResultInfo<TResult>(TResult result, string message)
        {
            var operation = OperationResult.CreateResult<TResult>();
            operation.AddInfo(message);
            operation.Result = result;
            return OperationResultResponse(OperationResultBeforeReturn(operation));
        }

        /// <summary>
        /// OperationResult Response
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [NonAction]
        protected ActionResult<OperationResult<TResult>> OperationResultSuccess<TResult>(TResult result)
        {
            var operation = OperationResult.CreateResult<TResult>();
            operation.Result = result;
            return OperationResultResponse(OperationResultBeforeReturn(operation));
        }

        /// <summary>
        /// OperationResult Response
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        protected ActionResult<OperationResult<TResult>> OperationResultSuccess<TResult>(TResult result, string message)
        {
            var operation = OperationResult.CreateResult<TResult>();
            operation.AddSuccess(message);
            operation.Result = result;
            return OperationResultResponse(OperationResultBeforeReturn(operation));
        }

        /// <summary>
        /// OperationResult Response
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        protected ActionResult<OperationResult<TResult>> OperationResultWarning<TResult>(TResult result, string message)
        {
            var operation = OperationResult.CreateResult<TResult>();
            operation.AddWarning(message);
            operation.Result = result;
            return OperationResultResponse(OperationResultBeforeReturn(operation));
        }

        /// <summary>
        /// OperationResult Response
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [NonAction]
        protected ActionResult<OperationResult<TResult>> OperationResultError<TResult>(TResult result, string message)
        {
            var operation = OperationResult.CreateResult<TResult>();
            operation.AddError(message);
            operation.Result = result;
            return OperationResultResponse(OperationResultBeforeReturn(operation));
        }

        /// <summary>
        /// OperationResult Response
        /// </summary>
        /// <param name="result"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        [NonAction]
        protected ActionResult<OperationResult<TResult>> OperationResultError<TResult>(TResult result, Exception exception = null)
        {
            var operation = OperationResult.CreateResult<TResult>();
            operation.AddError(exception);
            operation.Result = result;
            return OperationResultResponse(operation);
        }
        /// <summary>
        /// OperationResultBeforeReturn OperationResult after it has been created and filled
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="operationResult"></param>
        /// <returns></returns>
        [NonAction]
        protected virtual OperationResult<TResult> OperationResultBeforeReturn<TResult>(OperationResult<TResult> operationResult)
        {
            return operationResult;
        }

        #region OnCreateAndEdit Handlers

        /// <summary>
        /// Return current user name from Request Identity or from override code
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public virtual string GetUserIdentityFromRequest()
        {
            var user = User?.Identity?.Name;
            return user ?? _anonymousName;
        }


        #endregion
    }
}
