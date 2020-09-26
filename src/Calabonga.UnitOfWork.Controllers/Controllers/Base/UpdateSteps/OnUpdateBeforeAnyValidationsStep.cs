namespace Calabonga.UnitOfWork.Controllers.Controllers.Base.UpdateSteps
{
    /// <summary>
    /// On UpdateBeforeAnyValidations Step
    /// </summary>
    public class OnUpdateBeforeAnyValidationsStep: PipelineStep
    {
        /// <inheritdoc />
        public OnUpdateBeforeAnyValidationsStep(IWritableContext context)
        {
            Context = context;
        }

        /// <inheritdoc />
        protected override int OrderIndex => 2;
    }
}