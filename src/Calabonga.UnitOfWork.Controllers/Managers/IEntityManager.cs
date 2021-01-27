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
    /// Interface of the EntityManager
    /// </summary>
    public interface IEntityManager
    {
        /// <summary>
        /// Current manager name
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Entity Manager
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TCreateViewModel"></typeparam>
    /// <typeparam name="TUpdateViewModel"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public interface IEntityManager<TViewModel, TEntity, TCreateViewModel, TUpdateViewModel> : IEntityManager
        where TCreateViewModel : IViewModel, new()
        where TUpdateViewModel : ViewModelBase, new()
        where TViewModel : ViewModelBase, new()
        where TEntity : class, IHaveId
    {
        #region Properties

        /// <summary>
        /// User from request
        /// </summary>
        IIdentity? Principal { get; }

        /// <summary>
        /// <see cref="IMapper"/> instance for current manager
        /// </summary>
        IMapper CurrentMapper { get; }

        /// <summary>
        /// ViewModel factory can help you to instantiate view models with initial data
        /// </summary>
        IViewModelFactory<TCreateViewModel, TUpdateViewModel> ViewModelFactory { get; }

        /// <summary>
        /// Entity validator for current entity type. It can contains custom set of rules for entity validations
        /// </summary>
        IEntityValidator<TEntity> Validator { get; }

        #endregion

        #region OnCreate Handlers

        /// <summary>
        /// Fires before any mapping operation.
        /// This is first step for Create pipeline
        /// OrderIndex 1
        /// </summary>
        /// <returns></returns>
        Task<PipelineStep> OnCreateBeforeMappingAsync(PipelineStep step);

        /// <summary>
        /// Fires before entity validation executed on entity creation
        /// This is second step for Create pipeline
        /// OrderIndex 2
        /// </summary>
        /// <param name="step"></param>
        Task<PipelineStep> OnCreateBeforeAnyValidationsAsync(PipelineStep step);

        /// <summary>
        /// Fires when entity ready to Insert but some operations still need to do before saveChanges executed
        /// This is third step for Create pipeline
        /// OrderIndex 3
        /// </summary>
        /// <param name="step"></param>
        Task<PipelineStep> OnCreateBeforeSaveChangesAsync(PipelineStep step);

        /// <summary>
        /// Fires after changes successfully saved
        /// This is fourth step for Create pipeline
        /// OrderIndex 4
        /// </summary>
        /// <param name="step"></param>
        Task<PipelineResult> OnCreateAfterSaveChangesSuccessAsync(PipelineResult step);

        /// <summary>
        /// Fires after SaveChanges failed. In following after step 3.
        /// This is fifth step for Create pipeline.
        /// OrderIndex 5
        /// </summary>
        /// <param name="step"></param>
        Task<PipelineResult> OnCreateAfterSaveChangesFailedAsync(PipelineResult step);

        #endregion

        #region OnUpdate Handlers

        /// <summary>
        /// Fires when validation already complete and next step is saving entity on update operations
        /// </summary>
        /// <param name="step"></param>
        Task<PipelineStep> OnUpdateBeforeSaveChangesAsync(PipelineStep step);

        /// <summary>
        /// Fires before entity validation executed on entity updating
        /// </summary>
        /// <param name="step"></param>
        Task<PipelineStep> OnUpdateBeforeAnyValidationsAsync(PipelineStep step);

        /// <summary>
        /// Fires before any mapping operations begin
        /// </summary>
        /// <param name="step"></param>
        Task<PipelineStep> OnUpdateBeforeMappingsAsync(PipelineStep step);

        /// <summary>
        /// Fires after changes successfully saved
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        Task<PipelineResult> OnUpdateAfterSaveChangesSuccessAsync(PipelineResult step);

        /// <summary>
        /// Fires after SaveChanges failed
        /// </summary>
        /// <param name="step"></param>
        Task<PipelineResult> OnUpdateAfterSaveChangesFailedAsync(PipelineResult step);

        #endregion

        /// <summary>
        /// Update information for IAuditable entity
        /// </summary>
        /// <param name="entity"></param>
        void SetAuditInformation(TEntity entity);
    }
}