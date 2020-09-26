namespace Calabonga.UnitOfWork.Controllers.Controllers.Base.UpdateSteps
{
    /// <summary>
    /// Update step 1
    /// </summary>
    public class OnUpdateBeforeMappingStep: PipelineStep
    {
        /// <inheritdoc />
        public OnUpdateBeforeMappingStep(IWritableContext context)
        {
            Context = context;
        }

        /// <inheritdoc />
        protected override int OrderIndex => 1;
    }
}