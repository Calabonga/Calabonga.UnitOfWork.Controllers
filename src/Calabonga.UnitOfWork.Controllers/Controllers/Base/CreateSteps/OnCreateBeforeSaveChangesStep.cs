namespace Calabonga.UnitOfWork.Controllers.Controllers.Base.CreateSteps
{
    /// <summary>
    /// Step for Create pipeline 3
    /// </summary>
    public class OnCreateBeforeSaveChangesStep: PipelineStep
    {
        /// <inheritdoc />
        public OnCreateBeforeSaveChangesStep(IWritableContext context)
        {
            Context = context;
        }

        /// <inheritdoc />
        protected override int OrderIndex => 3;
    }
}