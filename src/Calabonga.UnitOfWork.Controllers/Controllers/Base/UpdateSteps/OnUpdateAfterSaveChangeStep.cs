namespace Calabonga.UnitOfWork.Controllers.Controllers.Base.UpdateSteps
{
    /// <summary>
    /// On UpdateAfterSaveChange Step
    /// </summary>
    public class OnUpdateAfterSaveChangeStep: PipelineResult
    {
        /// <inheritdoc />
        public OnUpdateAfterSaveChangeStep(IWritableContext context)
        {
            Context = context;
        }

        /// <inheritdoc />
        protected override int OrderIndex => 4;
    }
}