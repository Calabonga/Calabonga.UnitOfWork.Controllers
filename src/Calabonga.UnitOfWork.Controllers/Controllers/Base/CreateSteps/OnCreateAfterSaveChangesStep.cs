namespace Calabonga.UnitOfWork.Controllers.Controllers.Base.CreateSteps
{
    /// <summary>
    /// Step for Create pipeline 4
    /// </summary>
    public class OnCreateAfterSaveChangesStep: PipelineResult
    {
        /// <inheritdoc />
        public OnCreateAfterSaveChangesStep(IWritableContext context)
        {
            Context = context;
        }

        /// <inheritdoc />
        protected override int OrderIndex => 4;
    }
}