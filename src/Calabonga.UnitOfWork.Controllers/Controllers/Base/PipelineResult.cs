namespace Calabonga.UnitOfWork.Controllers.Controllers.Base
{
    /// <summary>
    /// Result of the pipeline
    /// </summary>
    public class PipelineResult: PipelineStep
    {
        protected PipelineResult()
        {
            // Result pipeline should stop process
            Stop();
        }

        /// <inheritdoc />
        protected override int OrderIndex => 6;
    }
}