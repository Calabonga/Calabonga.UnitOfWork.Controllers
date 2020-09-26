using System;

namespace Calabonga.UnitOfWork.Controllers.Controllers.Base
{
    /// <summary>
    /// Pipeline step interface
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        /// Flag indicate that pipeline should be stopped
        /// </summary>
        bool IsStopped { get; }

        /// <summary>
        /// Stop pipeline and return OperationResult
        /// </summary>
        void Stop();

        /// <summary>
        /// Stop pipeline and return OperationResult
        /// </summary>
        /// <param name="exception">exception for operation result error</param>
        /// <param name="dataObject"></param>
        void StopWithError<T>(Exception exception, object dataObject = null);

        /// <summary>
        /// Stop pipeline and return OperationResult
        /// </summary>
        /// <param name="errorMessage">exception for operation result error</param>
        /// <param name="dataObject"></param>
        void StopWithError<T>(string errorMessage, object dataObject = null);

        /// <summary>
        /// Stop pipeline and return OperationResult
        /// </summary>
        /// <param name="warningMessage"></param>
        /// <param name="dataObject"></param>
        void StopWithWarning<T>(string warningMessage, object dataObject = null);

        /// <summary>
        /// Stop pipeline and return OperationResult
        /// </summary>
        /// <param name="successMessage"></param>
        /// <param name="dataObject"></param>
        void StopWithSuccess<T>(string successMessage, object dataObject = null);

        /// <summary>
        /// Stop pipeline and return OperationResult
        /// </summary>
        /// <param name="infoMessage"></param>
        /// <param name="dataObject"></param>
        void StopWithInfo<T>(string infoMessage, object dataObject = null);
    }
}