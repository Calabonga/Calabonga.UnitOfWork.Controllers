namespace Calabonga.UnitOfWork.Controllers.Controllers.Base.UpdateSteps
{
    /// <summary>
    /// On UpdateBeforeSaveChanges Step
    /// </summary>
    public class OnUpdateBeforeSaveChangesStep: PipelineStep
    {
        /// <inheritdoc />
        public OnUpdateBeforeSaveChangesStep(IWritableContext context)
        {
            Context = context;
        }

        /// <inheritdoc />
        protected override int OrderIndex => 3;
    }
}