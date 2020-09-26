namespace Calabonga.UnitOfWork.Controllers.Controllers
{
    /// <summary>
    /// Unit Of Work controller
    /// </summary>
    public abstract class UnitOfWorkController : OperationResultController
    {
        /// <inheritdoc />
        protected UnitOfWorkController(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// Current Unit Of Work 
        /// </summary>
        protected IUnitOfWork UnitOfWork { get; set; }
    }
}