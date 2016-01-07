namespace UnityExtensions
{
    using System;
    using System.Configuration;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Lifetime manager which reloads whenever the AppSettings value changes.
    /// This should be used in Azure asp .net applications which may have different
    /// app settings after a vip swap.
    /// For example, if in a staging slot your application uses a different connection
    /// string than what is in production, you can have a container controlled lifetime
    /// manager based instance of a sql connection.  After the swap, this instance will
    /// be properly updated if usin this lifetime manager.
    /// </summary>
    public class ConfigurationChangeLifetimeManager : ContainerControlledLifetimeManager
    {
        private readonly string configValueKey;
        private string slotKeyValue;

        public ConfigurationChangeLifetimeManager(string configValueKey)
        {
            if (string.IsNullOrEmpty(configValueKey))
            {
                throw new ArgumentNullException(nameof(configValueKey));
            }

            this.configValueKey = configValueKey;
            this.slotKeyValue = this.GetSlotKey();
        }

        protected override object SynchronizedGetValue()
        {
            var currentSlotKey = this.GetSlotKey();
            if (this.slotKeyValue != currentSlotKey)
            {
                // refresh it
                this.slotKeyValue = currentSlotKey;
                return null;
            }
            else
            {
                return base.SynchronizedGetValue();
            }
        }

        private string GetSlotKey()
        {
            string slotKey = ConfigurationManager.AppSettings[this.configValueKey];
            return slotKey;
        }
    }
}