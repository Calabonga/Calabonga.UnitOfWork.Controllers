using System;
using Calabonga.EntityFrameworkCore.Entities.Base;

namespace Calabonga.UnitOfWork.Controllers
{
    /// <summary>
    /// Audit-able View Model Base
    /// </summary>
    public abstract class AuditableViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Created at datetime
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Author of the creation
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Update at datetime
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Author of the updates
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Delete at datetime
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Author of the deletion
        /// </summary>
        public string DeletedBy { get; set; }

        /// <summary>
        /// Archived at datetime
        /// </summary>
        public DateTime? ArchivedAt { get; set; }

        /// <summary>
        /// Author of the archiving
        /// </summary>
        public string ArchivedBy { get; set; }
    }
}
