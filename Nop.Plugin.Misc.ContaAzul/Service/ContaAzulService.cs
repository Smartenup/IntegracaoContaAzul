using Nop.Core.Data;
using Nop.Plugin.Misc.ContaAzul.Domain;
using Nop.Plugin.Misc.ContaAzul.Lib;
using Nop.Plugin.Misc.ContaAzul.Models.Message.Response;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ContaAzul.Service
{
    public class ContaAzulService: IContaAzulService
    {
        private readonly ICustomerService _customerService;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;


        public ContaAzulService(ICustomerService customerService, ISettingService settingService, ILogger logger)
        {
            _customerService = customerService;
            _settingService = settingService;
            _logger = logger;
        }

        /// <summary>
        /// Logs the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        //public void Log()
        //{
        //   // _contaAzulRecordRepository.Insert(record);
        //}

        public void RefreshToken()
        {
            //   var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var contaAzulMiscSettings = _settingService.LoadSetting<ContaAzulMiscSettings>();

            string username = contaAzulMiscSettings.client_id;
            string password = contaAzulMiscSettings.client_secret;

            TokenResponse tokenResponse = null;

            try
            {
                using (var token = new RefreshToken(contaAzulMiscSettings.UseSandbox))
                    tokenResponse = token.CreateAsync(username, password, contaAzulMiscSettings.refresh_token).ConfigureAwait(false).GetAwaiter().GetResult();

                contaAzulMiscSettings.access_token = tokenResponse.AcessToken;
                contaAzulMiscSettings.refresh_token = tokenResponse.RefreshToken;

                _settingService.SaveSetting(contaAzulMiscSettings);

                //now clear settings cache
                _settingService.ClearCache();
            }
            catch (Exception ex)
            {
                //  ErrorNotification(ex.Message);
                _logger.Error(ex.Message, ex);
            }
        }


    }
}
