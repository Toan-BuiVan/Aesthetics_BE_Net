using Aesthetics.Data.AestheticsInterfaces;
using Aesthetics.Data.RepositoryInterfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Data.AestheticsServices
{
    public class CustomerTreatmentSessionsService : ICustomerTreatmentSessionsService
	{
		private readonly ILogger<CustomerTreatmentSessionsService> _logger;
		private readonly ICustomerTreatmentSessionsRepository _customerTreatmentSessionsRepository;

		public CustomerTreatmentSessionsService(ILogger<CustomerTreatmentSessionsService> logger, ICustomerTreatmentSessionsRepository customerTreatmentSessionsRepository)
		{
			_logger = logger;
			_customerTreatmentSessionsRepository = customerTreatmentSessionsRepository;
		}
	}
}
