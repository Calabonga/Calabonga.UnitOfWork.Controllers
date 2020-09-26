using System;

namespace Calabonga.UnitOfWork.Controllers.Controllers.Base
{
    /// <summary>
    /// All step for modification are returns this type of the operation
    /// </summary>
    public abstract class PipelineStep: IPipeline
    {
        /// <inheritdoc />
        protected PipelineStep() { }

        /// <summary>
        /// Writable controller context
        /// </summary>
        public IWritableContext Context { get; set; }

        /// <summary>
        /// Pipeline operation executing order index
        /// </summary>
        protected abstract int OrderIndex { get; }

        /// <summary>
        /// Returns true when developer need to stop other operation and return result
        /// </summary>
        public bool IsStopped { get; private set; }

        /// <summary>
        /// Stop pipeline
        /// </summary>
        public void Stop()
        {
            IsStopped = true;
        }

        /// <inheritdoc />
        public void StopWithError<T>(Exception exception, object dataObject = null)
        {
            Context.AddError<T>(exception, dataObject);
            IsStopped = true;
        }

        /// <inheritdoc />
        public void StopWithError<T>(string errorMessage, object dataObject = null)
        {
            Context.AddError<T>(errorMessage, dataObject);
            IsStopped = true;
        }

        /// <inheritdoc />
        public void StopWithWarning<T>(string warningMessage, object dataObject = null)
        {
            Context.AddWarning<T>(warningMessage, dataObject);
            IsStopped = true;
        }

        /// <inheritdoc />
        public void StopWithSuccess<T>(string successMessage, object dataObject = null)
        {
            Context.AddSuccess<T>(successMessage, dataObject);
            IsStopped = true;
        }

        /// <inheritdoc />
        public void StopWithInfo<T>(string infoMessage, object dataObject = null)
        {
            Context.AddInfo<T>(infoMessage, dataObject);
            IsStopped = true;
        }
    }
}

