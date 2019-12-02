using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InterviewTask.Models;

namespace InterviewTask.Services
{
    public class HelperServiceRepository : IHelperServiceRepository
    {
        /// <summary>
        /// Returns all HelperService data, form the back-office CRM system.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HelperServiceModel> Get()
        {
            return HelperServiceFactory.Create();
        }

        /// <summary>
        /// Returns a single HelperService from the back-office CRM system.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public HelperServiceModel Get(Guid id)
        {
            return HelperServiceFactory.Create().FirstOrDefault(g => g.Id == id);
        }
    }
}