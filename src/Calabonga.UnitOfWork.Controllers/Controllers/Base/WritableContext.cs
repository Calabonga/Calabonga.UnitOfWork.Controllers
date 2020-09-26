using System;
using System.Collections.Generic;
using Calabonga.EntityFrameworkCore.Entities.Base;
using Calabonga.OperationResultsCore;
using Calabonga.UnitOfWork.Controllers.Exceptions;
using Calabonga.UnitOfWork.Controllers.Helpers;

namespace Calabonga.UnitOfWork.Controllers.Controllers.Base
{
    /// <summary>
    /// Writable controller context implementation
    /// </summary>
    public class WritableContext : ParamsProperty, IWritableContext
    {
        private const string OperationResultKeyName = "OperationResult";
        private const string ViewModelKeyName = "ViewModel";
        private const string CreateModelKeyName = "CreateViewModel";
        private const string UpdateModelKeyName = "UpdateViewModel";
        private const string EntityKeyName = "Entity";

        /// <inheritdoc />
        public void InitOrUpdate<TViewModel>(OperationResult<TViewModel> operationResult = null)
        {
            if (operationResult == null)
            {
                operationResult = OperationResult.CreateResult<TViewModel>();
            }
            AddOrUpdateParameter(OperationResultKeyName, operationResult);
        }

        #region AddError

        /// <inheritdoc />
        public void AddError<TViewModel>(Exception exception, object dataObject = null)
        {
            var operationResult = GetParamByName<OperationResult<TViewModel>>(OperationResultKeyName);
            if (operationResult == null)
            {
                throw new UowArgumentNullException(OperationResultKeyName);
            }

            if (dataObject == null)
            {
                operationResult.AddError(exception);
            }
            else
            {
                operationResult.AddError(exception).AddData(dataObject);
            }
            InitOrUpdate(operationResult);
        }

        /// <inheritdoc />
        public void AddError<T>(string errorMessage, object dataObject = null)
        {
            var operationResult = GetParamByName<OperationResult<T>>(OperationResultKeyName);
            if (operationResult == null)
            {
                throw new UowArgumentNullException(OperationResultKeyName);
            }

            if (dataObject == null)
            {
                operationResult.AddError(errorMessage);
            }
            else
            {
                operationResult.AddError(errorMessage).AddData(dataObject);
            }
            InitOrUpdate(operationResult);
        }

        #endregion

        #region AddWarning

        /// <inheritdoc />
        public void AddWarning<TViewModel>(string warningMessage, object dataObject = null)
        {
            var operationResult = GetParamByName<OperationResult<TViewModel>>(OperationResultKeyName);
            if (operationResult == null)
            {
                throw new UowArgumentNullException(OperationResultKeyName);
            }

            if (dataObject == null)
            {
                operationResult.AddWarning(warningMessage);
            }
            else
            {
                operationResult.AddWarning(warningMessage).AddData(dataObject);
            }
            InitOrUpdate(operationResult);
        }

        #endregion

        #region AddSuccess

        /// <inheritdoc />
        public void AddSuccess<TViewModel>(string successMessage, object dataObject = null)
        {
            var operationResult = GetParamByName<OperationResult<TViewModel>>(OperationResultKeyName);
            if (operationResult == null)
            {
                throw new UowArgumentNullException(OperationResultKeyName);
            }

            if (dataObject == null)
            {
                operationResult.AddSuccess(successMessage);
            }
            else
            {
                operationResult.AddSuccess(successMessage).AddData(dataObject);
            }
            InitOrUpdate(operationResult);
        }

        #endregion

        #region AddInfo

        /// <inheritdoc />
        public void AddInfo<TViewModel>(string infoMessage)
        {
            var operationResult = GetParamByName<OperationResult<TViewModel>>(OperationResultKeyName);
            if (operationResult == null)
            {
                throw new UowArgumentNullException(OperationResultKeyName);
            }

            operationResult.AddInfo(infoMessage);
            InitOrUpdate(operationResult);
        }

        /// <inheritdoc />
        public void AddInfo<TViewModel>(string infoMessage, object dataObject)
        {
            var operationResult = GetParamByName<OperationResult<TViewModel>>(OperationResultKeyName);
            if (operationResult == null)
            {
                throw new UowArgumentNullException(OperationResultKeyName);
            }

            operationResult.AddInfo(infoMessage).AddData(dataObject);
            InitOrUpdate(operationResult);
        }

        #endregion

        /// <inheritdoc />
        public void AddOrUpdateParameter(string name, object value)
        {
            base.AddOrUpdateParameter(name, value);
        }

        /// <inheritdoc />
        public TEntity GetEntity<TEntity>()
        {
            var entity = GetParamByName<TEntity>(EntityKeyName);
            if (entity == null)
            {
                throw new UowArgumentNullException(EntityKeyName);
            }
            return entity;
        }

        /// <inheritdoc />
        public TCreateViewModel GetCreateViewModel<TCreateViewModel>()
        {
            var createViewModel = GetParamByName<TCreateViewModel>(CreateModelKeyName);
            if (createViewModel == null)
            {
                throw new UowArgumentNullException(CreateModelKeyName);
            }

            return createViewModel;
        }

        /// <inheritdoc />
        public TUpdateViewModel GetUpdateViewModel<TUpdateViewModel>()
        {
            var updateViewModel = GetParamByName<TUpdateViewModel>(UpdateModelKeyName);
            if (updateViewModel == null)
            {
                throw new UowArgumentNullException(UpdateModelKeyName);
            }

            return updateViewModel;
        }

        /// <inheritdoc />
        public void AppendLog<T>(string message)
        {
            var operationResult = GetOperationResult<T>();
            operationResult.AppendLog(message);
            InitOrUpdate(operationResult);
        }

        /// <inheritdoc />
        public void AppendLog<T>(IEnumerable<string> logs)
        {
            var operationResult = GetOperationResult<T>();
            operationResult.AppendLog(logs);
            InitOrUpdate(operationResult);
        }

        /// <inheritdoc />
        public T GetResult<T>()
        {
            var operationResult = GetOperationResult<T>();
            if (operationResult.Result==null)
            {
                throw new UowArgumentNullException(ViewModelKeyName);
            }
            return operationResult.Result;
        }

        /// <inheritdoc />
        public void SetResult<T>(T result)
        {
            var operationResult = GetOperationResult<T>();
            operationResult.Result = result;
            InitOrUpdate(operationResult);
        }

        /// <inheritdoc />
        public string GetMessage<T>()
        {
            var operationResult = GetOperationResult<T>();
            return operationResult.Metadata?.Message;
        }

        /// <summary>
        /// Adds or Update instance for Entity 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public void AddOrUpdateEntity<TEntity>(TEntity entity) where TEntity : Identity
        {
            AddOrUpdateParameter(EntityKeyName, entity);
        }

        /// <summary>
        ///  Adds or update parameter UpdateViewModel in the Context of the WritableController
        /// </summary>
        /// <typeparam name="TUpdateViewModel"></typeparam>
        /// <param name="model"></param>
        internal void AddOrUpdateUpdateViewModel<TUpdateViewModel>(TUpdateViewModel model) where TUpdateViewModel : ViewModelBase, IHaveId, new()
        {
            AddOrUpdateParameter(UpdateModelKeyName, model);
        }

        /// <summary>
        ///  Adds or update parameter CreateViewModel in the Context of the WritableController
        /// </summary>
        /// <typeparam name="TCreateViewModel"></typeparam>
        /// <param name="model"></param>
        internal void AddOrUpdateCreateViewModel<TCreateViewModel>(TCreateViewModel model) where TCreateViewModel : class, IViewModel, new()
        {
            AddOrUpdateParameter(CreateModelKeyName, model);
        }

        /// <summary>
        /// Returns OperationResult
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public OperationResult<T> GetOperationResult<T>()
        {
            var operationResult = GetParamByName<OperationResult<T>>(OperationResultKeyName);
            if (operationResult == null)
            {
                throw new UowArgumentNullException(OperationResultKeyName);
            }

            return operationResult;
        }
    }


}