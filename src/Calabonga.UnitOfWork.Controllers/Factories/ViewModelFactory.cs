using System;
using System.Threading.Tasks;
using Calabonga.EntityFrameworkCore.Entities.Base;
using Calabonga.OperationResultsCore;

namespace Calabonga.UnitOfWork.Controllers.Factories
{
    /// <summary>
    /// ViewModelFactory base class
    /// </summary>
    /// <typeparam name="TUpdateViewModel"></typeparam>
    /// <typeparam name="TCreateViewModel"></typeparam>
    public abstract class ViewModelFactory<TCreateViewModel, TUpdateViewModel> : IViewModelFactory<TCreateViewModel, TUpdateViewModel>
        where TCreateViewModel : IViewModel, new()
        where TUpdateViewModel : IViewModel, new()
    {
        /// <inheritdoc />
        public abstract Task<OperationResult<TCreateViewModel>> GenerateForCreateAsync();

        /// <inheritdoc />
        public abstract Task<OperationResult<TUpdateViewModel>> GenerateForUpdateAsync(Guid id);
    }
}