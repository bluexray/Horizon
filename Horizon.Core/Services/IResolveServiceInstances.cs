using Horizon.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Horizon.Core.Services
{
    public interface IResolveServiceInstances
    {
        Task<IList<ServiceInformation>> FindServiceInstancesAsync();
        Task<IList<ServiceInformation>> FindServiceInstancesAsync(string name);
        Task<IList<ServiceInformation>> FindServiceInstancesWithVersionAsync(string name, string version);
        Task<IList<ServiceInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> nameTagsPredicate,
            Predicate<ServiceInformation> ServiceInformationPredicate);
        Task<IList<ServiceInformation>> FindServiceInstancesAsync(Predicate<KeyValuePair<string, string[]>> predicate);
        Task<IList<ServiceInformation>> FindServiceInstancesAsync(Predicate<ServiceInformation> predicate);
        Task<IList<ServiceInformation>> FindAllServicesAsync();
    }
}
