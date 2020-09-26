using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Calabonga.EntityFrameworkCore.Entities.Base;
using Calabonga.Microservices.Core;
using Calabonga.Microservices.Core.Exceptions;
using Calabonga.Microservices.Core.Extensions;
using Calabonga.Microservices.Core.QueryParams;
using Calabonga.Microservices.Core.Validators;
using Calabonga.OperationResults;
using Calabonga.UnitOfWork.Controllers.Controllers.Base;
using Calabonga.UnitOfWork.Controllers.Controllers.Base.CreateSteps;
using Calabonga.UnitOfWork.Controllers.Controllers.Base.UpdateSteps;
using Calabonga.UnitOfWork.Controllers.Factories;
using Calabonga.UnitOfWork.Controllers.Managers;
using Microsoft.AspNetCore.Mvc;

namespace Calabonga.UnitOfWork.Controllers.Controllers
{
    /// <summary>
    /// Writable controller with predefined CRUD operations
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TCreateViewModel"></typeparam>
    /// <typeparam name="TUpdateViewModel"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TQueryParams"></typeparam>
    public abstract class WritableController<TViewModel, TEntity, TCreateViewModel, TUpdateViewModel, TQueryParams>
        : ReadOnlyController<TEntity, TViewModel, TQueryParams>
        where TEntity : Identity
        where TViewModel : ViewModelBase, new()
        where TQueryParams : PagedListQueryParams
        where TCreateViewModel : class, IViewModel, new()
        where TUpdateViewModel : ViewModelBase, IHaveId, new()
    {
        private WritableContext Context { get; }
        private IEntityManager<TViewModel, TEntity, TCreateViewModel, TUpdateViewModel> _currentEntityManager;

        /// <inheritdoc />
        protected WritableController(
            IEntityManagerFactory entityManagerFactory,
            IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(unitOfWork, mapper)
        {
            EntityManagerFactory = entityManagerFactory;
            Context = new WritableContext();
        }

        #region Properties

        public bool IsAutoHistoryEnabled { get; set; } = false;

        /// <summary>
        /// EntityManagerFactory instance
        /// </summary>
        protected IEntityManagerFactory EntityManagerFactory { get; }

        /// <summary>
        /// EntityManager default instance
        /// </summary>
        protected IEntityManager<TViewModel, TEntity, TCreateViewModel, TUpdateViewModel> EntityManager
        {
            get
            {
                if (_currentEntityManager == null)
                {
                    SetEntityManager();
                }
                return _currentEntityManager;
            }
        }

        #endregion

        #region PostAsync

        /// <summary>
        /// Returns predefined ViewModel for entity create operation.
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public virtual async Task<ActionResult<OperationResult<TCreateViewModel>>> GetViewmodelForCreation()
        {
            return await EntityManager.ViewModelFactory.GenerateForCreateAsync();
        }

        /// <summary>
        /// Creates entity. For viewModel generation you can use {get-create-viewmodel} method to
        /// get viewModel with predefined properties
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [ProducesResponseType(200)]
        public virtual async Task<ActionResult<OperationResult<TViewModel>>> PostItem(TCreateViewModel model)
        {
            Context.InitOrUpdate<TViewModel>();

            await using var transaction = await UnitOfWork.BeginTransactionAsync();
            Context.AddOrUpdateCreateViewModel(model);

            var step1 = new OnCreateBeforeMappingStep(Context);
            await EntityManager.OnCreateBeforeMappingAsync(step1);
            if (step1.IsStopped)
            {
                await transaction.RollbackAsync();
                return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
            }

            // Step 1: Mapping
            var entity = EntityManager.CurrentMapper.Map<TCreateViewModel, TEntity>(model);
            if (entity == null)
            {
                await transaction.RollbackAsync();
                Context.AddError<TViewModel>(new MicroserviceUnauthorizedException(AppContracts.Exceptions.MappingException));
                return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
            }

            // Step 2: Set audit information
            EntityManager.SetAuditInformation(entity);

            // Step 3: On create before validations
            Context.AddOrUpdateEntity(entity);
            var step3 = new OnCreateBeforeAnyValidationsStep(Context);
            await EntityManager.OnCreateBeforeAnyValidationsAsync(step3);
            if (step3.IsStopped)
            {
                await transaction.RollbackAsync();
                return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
            }

            // Step 4: On create before validations
            ValidateUserAccessRights(entity);
            if (EntityManager.Validator.IsNeedToStop)
            {
                await transaction.RollbackAsync();
                Context.AddError<TViewModel>(
                    new MicroserviceUnauthorizedException(EntityManager.Validator.ValidationContext.ToString()),
                    EntityManager.Validator.ValidationContext.Errors);

                return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
            }

            // Step 5: Validations
            var validatorResult = EntityManager.Validator
                .ValidateByOperationType(EntityValidationType.Insert, entity)
                .ToList()
                .GetResult();

            if (!validatorResult.IsValid)
            {
                await transaction.RollbackAsync();
                Context.AddError<TViewModel>(
                    new MicroserviceEntityValidationException(validatorResult.ToString()),
                    validatorResult.Errors);

                return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
            }

            // Step 6: Create Before SaveChanges
            var step6 = new OnCreateBeforeSaveChangesStep(Context);
            await EntityManager.OnCreateBeforeSaveChangesAsync(step6);
            if (step6.IsStopped)
            {
                await transaction.RollbackAsync();
                return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
            }

            await Repository.InsertAsync(entity);
            await UnitOfWork.SaveChangesAsync(IsAutoHistoryEnabled);

            var lastResult = UnitOfWork.LastSaveChangesResult;
            if (lastResult.IsOk)
            {
                // Step 7:
                var mapped = EntityManager.CurrentMapper.Map<TEntity, TViewModel>(entity);
                Context.SetResult(mapped);
                var step7 = new OnCreateAfterSaveChangesStep(Context);
                await EntityManager.OnCreateAfterSaveChangesSuccessAsync(step7);
                if (step7.IsStopped)
                {
                    await transaction.CommitAsync();
                    return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
                }
            }
            // Step 8:
            var step8 = new OnCreateAfterSaveChangesStep(Context);

            var operation = Context.GetOperationResult<TViewModel>();
            await EntityManager.OnCreateAfterSaveChangesFailedAsync(step8);
            if (step8.IsStopped)
            {
                if (operation.Exception == null)
                {
                    operation.AddError(lastResult.Exception);
                }
            }
            await transaction.RollbackAsync();
            return Ok(OperationResultBeforeReturn(operation));
        }

        #endregion

        #region PutAsync

        /// <summary>
        /// Returns predefined ViewModel for entity create operation.
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]/{id:guid}")]
        public virtual async Task<ActionResult<OperationResult<TUpdateViewModel>>> GetViewmodelForEditing(Guid id)
        {
            return await EntityManager.ViewModelFactory.GenerateForUpdateAsync(id);
        }

        /// <summary>
        /// Updates entity. For viewModel generation you can use {get-create-viewmodel} method to get viewModel with predefined properties
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("[action]/{id:guid}")]
        [ProducesResponseType(200)]
        public virtual async Task<ActionResult<OperationResult<TViewModel>>> PutItem(Guid id, TUpdateViewModel model)
        {
            Context.InitOrUpdate<TViewModel>();

            await using var transaction = await UnitOfWork.BeginTransactionAsync();
            Context.AddOrUpdateUpdateViewModel(model);

            // step 1: Find entity
            var entity = FindEntity(id, model);
            if (entity == null)
            {
                await transaction.RollbackAsync();
                Context.AddError<TViewModel>(new MicroserviceUnauthorizedException(AppContracts.Exceptions.NotFoundException));
                return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
            }

            Context.AddOrUpdateEntity(entity);

            // step 2: Set audit information
            EntityManager.SetAuditInformation(entity);

            // step 3
            var step1 = new OnUpdateBeforeMappingStep(Context);
            await EntityManager.OnUpdateBeforeMappingsAsync(step1);
            if (step1.IsStopped)
            {
                await transaction.RollbackAsync();
                return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
            }

            // step 4: Mapping
            EntityManager.CurrentMapper.Map(model, entity);

            var step2 = new OnUpdateBeforeAnyValidationsStep(Context);
            await EntityManager.OnUpdateBeforeAnyValidationsAsync(step2);
            if (step2.IsStopped)
            {
                await transaction.RollbackAsync();
                return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
            }

            // step 5: access validations
            ValidateUserAccessRights(entity);
            if (EntityManager.Validator.IsNeedToStop)
            {
                await transaction.RollbackAsync();
                Context.AddError<TViewModel>(
                    new MicroserviceEntityValidationException(EntityManager.Validator.ValidationContext.ToString()),
                    EntityManager.Validator.ValidationContext.Errors);

                return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
            }

            //step 6: other validations
            var validatorResult = EntityManager.Validator
                .ValidateByOperationType(EntityValidationType.Update, entity)
                .ToList()
                .GetResult();

            if (!validatorResult.IsValid)
            {
                await transaction.RollbackAsync();
                Context.AddError<TViewModel>(new MicroserviceEntityValidationException(validatorResult.ToString()), validatorResult.Errors);
                return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
            }

            var step3 = new OnUpdateBeforeSaveChangesStep(Context);
            await EntityManager.OnUpdateBeforeSaveChangesAsync(step3);
            if (step3.IsStopped)
            {
                await transaction.RollbackAsync();
                return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
            }

            Repository.Update(entity);
            await UnitOfWork.SaveChangesAsync(IsAutoHistoryEnabled);

            var lastResult = UnitOfWork.LastSaveChangesResult;
            if (lastResult.IsOk)
            {
                var mapped = EntityManager.CurrentMapper.Map<TEntity, TViewModel>(entity);
                Context.SetResult(mapped);
                var step4 = new OnUpdateAfterSaveChangeStep(Context);

                await EntityManager.OnUpdateAfterSaveChangesSuccessAsync(step4);
                if (step4.IsStopped)
                {
                    await transaction.CommitAsync();
                    return Ok(OperationResultBeforeReturn(Context.GetOperationResult<TViewModel>()));
                }
            }
            var step5 = new OnUpdateAfterSaveChangeStep(Context);

            var operation = Context.GetOperationResult<TViewModel>();
            await EntityManager.OnUpdateAfterSaveChangesFailedAsync(step5);
            if (!step5.IsStopped) return Ok(OperationResultBeforeReturn(operation));
            await transaction.RollbackAsync();
            if (operation.Exception == null)
            {
                operation.AddError(lastResult.Exception);
            }
            return Ok(OperationResultBeforeReturn(operation));
        }

        #endregion

        #region DeleteAsync
        /// <summary>
        /// Deletes entity from repository by identifier and return it as response
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("[action]/{id:guid}")]
        [ProducesResponseType(200)]
        public virtual async Task<ActionResult<OperationResult<TViewModel>>> DeleteItem(Guid id)
        {
            var operation = OperationResult.CreateResult<TViewModel>();
            var entity = await Repository.FindAsync(id);
            if (entity == null)
            {
                operation.AddError(new MicroserviceNotFoundException());
                return Ok(OperationResultBeforeReturn(operation));
            }

            var accessRights = ValidateUserAccessRights(entity);
            if (!accessRights.IsOk)
            {
                operation.AddError(new UnauthorizedAccessException(accessRights.ToString()));
                return Ok(OperationResultBeforeReturn(operation));
            }

            Repository.Delete(entity);
            await UnitOfWork.SaveChangesAsync(IsAutoHistoryEnabled);
            if (UnitOfWork.LastSaveChangesResult.IsOk)
            {
                operation.Result = EntityManager.CurrentMapper.Map<TEntity, TViewModel>(entity);
                return Ok(OperationResultBeforeReturn(operation));
            }
            operation.AddError(UnitOfWork.LastSaveChangesResult.Exception);
            return Ok(OperationResultBeforeReturn(operation));
        }

        #endregion

        #region Virtual and Absctract

        /// <summary>
        /// Returns default manager for current controller
        /// </summary>
        /// <param name="managers"></param>
        /// <returns></returns>
        protected virtual IEntityManager SelectEntityManager(IEnumerable<IEntityManager> managers)
        {
            return managers.First();
        }

        /// <summary>
        /// Returns entity from database by default without any includes
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected virtual TEntity FindEntity(Guid id, TUpdateViewModel model)
        {
            return Repository.Find(id);
        }

        #endregion

        #region Privates

        private void SetEntityManager()
        {
            if (EntityManagerFactory == null)
            {
                throw new MicroserviceInvalidOperationException("EntityManagerFactory is NULL");
            }

            if (!EntityManagerFactory.HasManagers)
            {
                throw new MicroserviceInvalidOperationException("EntityManagerFactory has not any managers registered");
            }

            if (EntityManagerFactory.Managers.Count() == 1)
            {
                _currentEntityManager = (IEntityManager<TViewModel, TEntity, TCreateViewModel, TUpdateViewModel>)EntityManagerFactory.Managers.First();
            }
            else
            {
                var managers = EntityManagerFactory.Managers.OfType<IEntityManager<TViewModel, TEntity, TCreateViewModel, TUpdateViewModel>>().ToList();
                if (managers.Any())
                {
                    var selected = SelectEntityManager(managers);
                    if (selected != null)
                    {
                        _currentEntityManager = (IEntityManager<TViewModel, TEntity, TCreateViewModel, TUpdateViewModel>)selected;
                    }
                    else
                    {
                        _currentEntityManager = (IEntityManager<TViewModel, TEntity, TCreateViewModel, TUpdateViewModel>)EntityManagerFactory.Managers.First();
                    }
                }
                else
                {
                    _currentEntityManager = (IEntityManager<TViewModel, TEntity, TCreateViewModel, TUpdateViewModel>)EntityManagerFactory.Managers.First();
                }
            }
        }

        #endregion
    }
}
