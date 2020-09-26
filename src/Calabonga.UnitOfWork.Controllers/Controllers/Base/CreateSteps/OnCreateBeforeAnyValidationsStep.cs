namespace Calabonga.UnitOfWork.Controllers.Controllers.Base.CreateSteps
{
    /// <summary>
    /// Step for Create pipeline 2
    /// </summary>
    public class OnCreateBeforeAnyValidationsStep: PipelineStep
        
    {
        /// <inheritdoc />
        public OnCreateBeforeAnyValidationsStep(IWritableContext context)
        {
            Context = context;
        }

        /// <inheritdoc />
        protected override int OrderIndex => 2;
    }
}