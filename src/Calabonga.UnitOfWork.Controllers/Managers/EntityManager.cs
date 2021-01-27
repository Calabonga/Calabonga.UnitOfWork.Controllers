using System;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Calabonga.EntityFrameworkCore.Entities.Base;
using Calabonga.Microservices.Core.Validators;
using Calabonga.UnitOfWork.Controllers.Controllers.Base;
using Calabonga.UnitOfWork.Controllers.Factories;

namespace Calabonga.UnitOfWork.Controllers.Managers
{
    /// <summary>
    /// Entity manager represents a set of the tools for entity management
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TUpdateViewModel"></typeparam>
    /// <typeparam name="TCreateViewModel"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public abstract class EntityManager<TViewModel, TEntity, TCreateViewModel, TUpdateViewModel>
        : IEntityManager<TViewModel, TEntity, TCreateViewModel, TUpdateViewModel>
        where TEntity : class, IHaveId
        where TCreateViewModel : IViewModel, new()
        where TUpdateViewModel : ViewModelBase, new()
        where TViewModel : ViewModelBase, new()
    {
        protected EntityManager(
            IMapper mapper,
            IViewModelFactory<TCreateViewModel, TUpdateViewModel> viewModelFactory,
            IEntityValidator<TEntity> validator)
        {
            CurrentMapper = mapper;
            ViewModelFactory = viewModelFactory;
            Validator = validator;
            Principal = GetIdentityInternal();
        }

        /// <summary>
        /// Set up user identity
        /// </summary>
        /// <returns></returns>
        protected abstract IIdentity? GetIdentity();

        /// <summary>
        /// Current manager name
        /// </summary>
        public virtual string Name => GetType().Name;

        /// <inheritdoc />
        public IIdentity Principal { get; }

        /// <summary>
        /// <see cref="IMapper"/> instance for current manager
        /// </summary>
        public IMapper CurrentMapper { get; }

        /// <summary>
        /// ViewModel factory can help you to instantiate view models with initial data
        /// </summary>
        public IViewModelFactory<TCreateViewModel, TUpdateViewModel> ViewModelFactory { get; }

        /// <summary>
        /// Entity validator for current entity type. It can contains custom set of rules for entity validations
        /// </summary>
        public IEntityValidator<TEntity> Validator { get; }

        public void SetAuditInformation(TEntity entity)
        {
            if (!(entity is IAuditable audible)) return;
            var auditUser = Principal?.Name;
            var date = DateTime.UtcNow;
            audible.CreatedBy = auditUser;
            audible.UpdatedBy = auditUser;
            audible.CreatedAt = date;
            audible.UpdatedAt = date;
        }

        #region OnCreated Handlers

        /// <inheritdoc />
        public virtual Task<PipelineStep> OnCreateBeforeMappingAsync(
            PipelineStep step)
        {
            return Task.FromResult(step);
        }

        /// <inheritdoc />
        public virtual Task<PipelineStep> OnCreateBeforeSaveChangesAsync(
            PipelineStep step)
        {
            return Task.FromResult(step);
        }

        /// <inheritdoc />
        public virtual Task<PipelineResult> OnCreateAfterSaveChangesSuccessAsync(PipelineResult step)
        {
            return Task.FromResult(step);
        }

        /// <inheritdoc />
        public virtual Task<PipelineStep> OnCreateBeforeAnyValidationsAsync(PipelineStep step)
        {
            return Task.FromResult(step);
        }

        /// <inheritdoc />
        public virtual Task<PipelineResult> OnCreateAfterSaveChangesFailedAsync(PipelineResult step)
        {
            return Task.FromResult(step);
        }

        #endregion

        #region OnEdit Handlers

        /// <inheritdoc />
        public virtual Task<PipelineResult> OnUpdateAfterSaveChangesFailedAsync(PipelineResult step)
        {
            return Task.FromResult(step);
        }

        /// <inheritdoc />
        public virtual Task<PipelineStep> OnUpdateBeforeSaveChangesAsync(
            PipelineStep step)
        {
            return Task.FromResult(step);
        }

        /// <inheritdoc />
        public virtual Task<PipelineStep> OnUpdateBeforeAnyValidationsAsync(
            PipelineStep step)
        {
            return Task.FromResult(step);
        }



        /// <inheritdoc />
        public virtual Task<PipelineStep> OnUpdateBeforeMappingsAsync(
            PipelineStep step)
        {
            return Task.FromResult(step);
        }

        /// <inheritdoc />
        public virtual Task<PipelineResult> OnUpdateAfterSaveChangesSuccessAsync(PipelineResult step)
        {
            return Task.FromResult(step);
        }

        #endregion

        private IIdentity? GetIdentityInternal()
        {
            return GetIdentity();
        }
    }
}