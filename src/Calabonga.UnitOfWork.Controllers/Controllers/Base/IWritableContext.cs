using System;
using System.Collections.Generic;
using Calabonga.EntityFrameworkCore.Entities.Base;
using Calabonga.OperationResultsCore;

namespace Calabonga.UnitOfWork.Controllers.Controllers.Base
{
    /// <summary>
    /// WritableContext interface
    /// </summary>
    public interface IWritableContext
    {
        /// <summary>
        /// Initiate new instance of the OperationResult or Update if exists
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        void InitOrUpdate<TViewModel>(OperationResult<TViewModel> operationResult = null);

        /// <summary>
        /// Add to OperationResult metadata message
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="exception"></param>
        /// <param name="dataObject"></param>
        void AddError<TViewModel>(Exception exception, object dataObject = null);

        /// <summary>
        /// Add to OperationResult metadata message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="errorMessage"></param>
        /// <param name="dataObject"></param>
        void AddError<T>(string errorMessage, object dataObject = null);

        /// <summary>
        /// Add to OperationResult metadata message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="warningMessage"></param>
        /// <param name="dataObject"></param>
        void AddWarning<T>(string warningMessage, object dataObject = null);

        /// <summary>
        /// Add to OperationResult metadata message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="successMessage"></param>
        /// <param name="dataObject"></param>
        void AddSuccess<T>(string successMessage, object dataObject = null);

        /// <summary>
        /// Add to OperationResult metadata message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="infoMessage"></param>
        void AddInfo<T>(string infoMessage);

        /// <summary>
        /// Add to OperationResult metadata message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="infoMessage"></param>
        /// <param name="dataObject"></param>
        void AddInfo<T>(string infoMessage, object dataObject);

        /// <summary>
        /// Add to writable context some object as parameter
        /// </summary>
        void AddOrUpdateParameter(string name, object value);

        /// <summary>
        /// Returns Entity model for current controller <see cref="TEntity"/>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        TEntity GetEntity<TEntity>();

        /// <summary>
        /// Adds or updates Entity model for current controller <see cref="TEntity"/>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        void AddOrUpdateEntity<TEntity>(TEntity entity) where TEntity : Identity;

        /// <summary>
        /// Creates CreateViewModel for current controller <see cref="TCreateViewModel"/>
        /// </summary>
        /// <typeparam name="TCreateViewModel"></typeparam>
        /// <returns></returns>
        TCreateViewModel GetCreateViewModel<TCreateViewModel>();

        /// <summary>
        /// Returns UpdateViewModel for current controller <see cref="TUpdateViewModel"/>
        /// </summary>
        /// <typeparam name="TUpdateViewModel"></typeparam>
        /// <returns></returns>
        TUpdateViewModel GetUpdateViewModel<TUpdateViewModel>();

        /// <summary>
        /// Returns Parameter from controller context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName"></param>
        /// <returns></returns>
        T GetParamByName<T>(string keyName);

        #region AppendLog

        /// <summary>
        /// Appends logs message into OperationResult in the WritableContext
        /// </summary>
        /// <param name="message"></param>
        void AppendLog<T>(string message);

        /// <summary>
        /// Appends logs message into OperationResult in the WritableContext
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logs"></param>
        void AppendLog<T>(IEnumerable<string> logs);
        
        #endregion

        /// <summary>
        /// Returns OperationResult Result entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetResult<T>();

        /// <summary>
        /// Sets the Result for OperationResult
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        void SetResult<T>(T result);


        /// <summary>
        /// Returns Message for OperationResult from context of the WritableController
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetMessage<T>();
        
        /// <summary>
        /// Returns OperationResult from context of the WritableController 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        OperationResult<T> GetOperationResult<T>();
    }



}