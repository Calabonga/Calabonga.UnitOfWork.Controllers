namespace Calabonga.UnitOfWork.Controllers.Controllers.Base.CreateSteps
{
    /// <summary>
    /// Step for Create pipeline 1
    /// </summary>
    public class OnCreateBeforeMappingStep: PipelineStep
        
    {
        /// <inheritdoc />
        public OnCreateBeforeMappingStep(IWritableContext context)
        {
            Context = context;
        }

        /// <inheritdoc />
        protected override int OrderIndex => 1;
    }
}