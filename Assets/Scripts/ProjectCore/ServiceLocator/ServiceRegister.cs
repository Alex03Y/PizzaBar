using System.Collections.Generic;
using System.Linq;
using ProjectCore.Misc;

namespace ProjectCore.ServiceLocator
{
    public class ServiceRegister : CachedBehaviour
    {
        public bool IncludeInactive = true;
        public SearchType ServiceSearchType;
        public enum SearchType
        {
            InDepthHierarchy,
            WholeObject,
            FirstEntry
        }

        private void Awake()
        {
            List<IService> findServices = null;
            switch (ServiceSearchType)
            {
                case SearchType.WholeObject:
                    findServices = new List<IService>(GetComponents<IService>());
                    break;
                case SearchType.InDepthHierarchy:
                    findServices = new List<IService>(GetComponentsInChildren<IService>(IncludeInactive));
                    break;
                case SearchType.FirstEntry:
                    findServices = new List<IService> {GetComponent<IService>()};
                    break;
            }

            findServices?.Where(x => x != null).ToList().ForEach(ServiceLocator.Register);
        }
    }
}