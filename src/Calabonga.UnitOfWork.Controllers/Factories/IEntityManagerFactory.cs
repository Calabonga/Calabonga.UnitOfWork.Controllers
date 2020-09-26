using System.Collections.Generic;
using Calabonga.UnitOfWork.Controllers.Managers;

namespace Calabonga.UnitOfWork.Controllers.Factories {
    /// <summary>
    /// EntityManager Factory interface
    /// </summary>
    public interface IEntityManagerFactory
    {
        IEntityManager EntityManager { get; }

        /// <summary>
        /// Returns total registered manager count for current types
        /// </summary>
        bool HasManagers { get; }

        // /// <summary>
        // /// Returns registered managers for current controller
        // /// </summary>
        IEnumerable<IEntityManager> Managers { get; }
    }
}