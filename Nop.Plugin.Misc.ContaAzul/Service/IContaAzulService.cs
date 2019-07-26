using Nop.Plugin.Misc.ContaAzul.Domain;

namespace Nop.Plugin.Misc.ContaAzul.Service
{
    public interface IContaAzulService
    {
        /// <summary>
        /// Logs the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        //void Log();

        void RefreshToken();

    }
}
