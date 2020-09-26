using System;
using System.Threading.Tasks;
using Calabonga.EntityFrameworkCore.Entities.Base;
using Calabonga.OperationResults;

namespace Calabonga.UnitOfWork.Controllers.Factories
{
    /// <summary>
    /// ViewModel Factory
    /// </summary>
    /// <typeparam name="TUpdateViewModel"></typeparam>
    /// <typeparam name="TCreateViewModel"></typeparam>
    public interface IViewModelFactory<TCreateViewModel, TUpdateViewModel>
        where TCreateViewModel : IViewModel, new()
        where TUpdateViewModel : IViewModel, new()
    {
        /// <summary>
        /// Returns ViewModel for entity creation
        /// </summary>
        /// <returns></returns>
        Task<OperationResult<TCreateViewModel>> GenerateForCreateAsync();

        /// <summary>
        /// Returns ViewModel for entity editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<OperationResult<TUpdateViewModel>> GenerateForUpdateAsync(Guid id);
    }
}