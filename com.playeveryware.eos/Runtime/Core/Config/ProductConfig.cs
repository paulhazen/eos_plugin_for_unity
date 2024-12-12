/*
 * Copyright (c) 2021 PlayEveryWare
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#if !EOS_DISABLE

namespace PlayEveryWare.EpicOnlineServices
{
    using Common;
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

#if !EXTERNAL_TO_UNITY
    using UnityEngine;
#endif

    /// <summary>
    /// Contains information about the product entered by the user from the Epic
    /// Developer Portal.
    /// </summary>
    [ConfigGroup("Product Configuration", new[] { "", "Deployment Configuration" }, false)]
    public class ProductConfig : Config
    {
        /// <summary>
        /// The product ID is a unique GUID labeled "Product ID" in the Epic
        /// Developer Portal. The name for this value can be set to anything -
        /// it is used as a label for user interface purposes - and is allowed
        /// to differ from the label given to it on the Developer Portal.
        /// </summary>
        [ConfigField("Product Name",
            ConfigFieldType.Text,
            "Enter your product name as it appears in the EOS Dev " +
            "Portal here.",
            0)]
        public string ProductName;

        [ConfigField("Product Id",
            ConfigFieldType.Guid,
            "Enter your Product Id as it appears in the EOS Dev " +
            "Portal here.",
            0)]
        public Guid ProductId;

        [ConfigField("Version",
            ConfigFieldType.Text,
            "Use this to indicate to the EOS SDK your game version.",
            0)]
        public string ProductVersion;

        /// <summary>
        /// This additional flag determines whether or not the productconfig has
        /// been imported. The reason that schema alone is not sufficient for
        /// this is because product_config is a new file altogether, so when it
        /// is first created, it will have the newest schema version, and
        /// migration will need to take place.
        /// </summary>
        [JsonProperty("imported")]
        private bool _configImported = false;

#if !EOS_DISABLE
        /// <summary>
        /// The set of Clients as defined within the Epic Developer Portal. For
        /// EOS to function, at least one of these must be set, and the
        /// platform config needs to indicate which one to use. (If none is
        /// explicitly indicated, and only one is defined, that one will be
        /// used).
        /// </summary>
        [ConfigField("Client Credentials",
            ConfigFieldType.SetOfClientCredentials,
            "Enter the client credentials you have defined in the " +
            "Epic Dev Portal.", 1)]
        public SetOfNamed<EOSClientCredentials> Clients = new("Client");
#endif

        /// <summary>
        /// The set of Sandboxes as defined within the Epic Developer Portal.
        /// For EOS to function, at least one of these must be set, and it must
        /// match the deployment indicated by the platform config.
        /// </summary>
        [ConfigField("Production Environments",
            ConfigFieldType.ProductionEnvironments,
            "Enter the details of your deployment and sandboxes as they " +
            "exist within the Epic Dev Portal.", 1)]
        public ProductionEnvironments Environments = new();

        static ProductConfig()
        {
            RegisterFactory(() => new ProductConfig());
        }

        protected override bool NeedsMigration()
        {
            return base.NeedsMigration() || !_configImported;
        }
        
        protected ProductConfig() : base("eos_product_config.json") { }

        #region Functionality to migrate from old configuration to new

        internal class PreviousEOSConfig : Config
        {
            public string productName;
            public string productVersion;
            public string productID;
            public List<SandboxDeploymentOverride> sandboxDeploymentOverrides;
            public string sandboxID;
            public string deploymentID;
            public string clientSecret;
            public string clientID;
            public string encryptionKey;

            static PreviousEOSConfig()
            {
                RegisterFactory(() => new PreviousEOSConfig());
            }

            protected PreviousEOSConfig() : base("EpicOnlineServicesConfig.json") { }
        }

        private void MigrateProductNameVersionAndId(PreviousEOSConfig config)
        {
            ProductName = config.productName;
            ProductVersion = config.productVersion;

            if (!Guid.TryParse(config.productID, out ProductId))
            {
                Debug.LogWarning("Could not parse product ID.");
            }
        }

        private void MigrateClientCredentials(PreviousEOSConfig config)
        {
#if !EOS_DISABLE
            // Import the old config client stuff
            Clients.Add(new EOSClientCredentials(config.clientID, config.clientSecret,
                config.encryptionKey));
#endif
        }

        private void MigrateSandboxAndDeployment(PreviousEOSConfig config)
        {
            // Import explicitly set sandbox and deployment
            SandboxId sandboxId = new()
            {
                Value = config.sandboxID
            };

            Deployment oldDeployment = new()
            {
                DeploymentId = Guid.Parse(config.deploymentID),
                SandboxId = sandboxId
            };

            if (!Environments.AddDeployment(oldDeployment))
            {
                Debug.LogWarning("Could not import deployment " +
                                 "details from old config file. Please " +
                                 "reach out for support if you need " +
                                 "assistance.");
            }
        }

        private void MigrateSandboxAndDeploymentOverrides(PreviousEOSConfig config)
        {
            // Import each of the overrides
            foreach (var overrideValues in config.sandboxDeploymentOverrides)
            {
                SandboxId overrideSandbox = new() { Value = overrideValues.sandboxID };
                Deployment overrideDeployment = new()
                {
                    DeploymentId = Guid.Parse(overrideValues.deploymentID),
                    SandboxId = overrideSandbox
                };

                Environments.Deployments.Add(overrideDeployment);
            }
        }

        protected override void MigrateConfig()
        {
            Environments ??= new();

            PreviousEOSConfig oldConfig = Get<PreviousEOSConfig>();

            MigrateProductNameVersionAndId(oldConfig);
            MigrateClientCredentials(oldConfig);
            MigrateSandboxAndDeployment(oldConfig);
            MigrateSandboxAndDeploymentOverrides(oldConfig);

            _configImported = true;
        }
        #endregion
    }
}

#endif