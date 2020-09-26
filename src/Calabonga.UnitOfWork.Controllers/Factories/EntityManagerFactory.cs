using System.Collections.Generic;
using System.Linq;
using Calabonga.UnitOfWork.Controllers.Managers;

namespace Calabonga.UnitOfWork.Controllers.Factories
{
    public class EntityManagerFactory : IEntityManagerFactory
    {
        public EntityManagerFactory(IEnumerable<IEntityManager> managers)
        {
            Managers = managers;
        }

        public IEntityManager EntityManager
        {
            get
            {
                if (Managers != null && Managers.Any())
                {
                    return Managers.First();
                }

                return null;
            }
        }

        /// <summary>
        /// Returns total registered manager count for current types
        /// </summary>
        public bool HasManagers
        {
            get
            {
                if (Managers == null) return false;
                return Managers.ToList().Any();
            }
        }

        public IEnumerable<IEntityManager> Managers { get; }
    }
}