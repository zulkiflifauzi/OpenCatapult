﻿// Copyright (c) Polyrific, Inc 2018. All rights reserved.

using System;

namespace Polyrific.Catapult.Shared.Dto.Provider
{
    public class ProviderDto
    {
        /// <summary>
        /// Id of the provider
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the provider
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of the provider
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Author of the provider
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Version of the provider
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Date when the provider was registered
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Required services of the provider
        /// </summary>
        public string[] RequiredServices { get; set; }

        /// <summary>
        /// Provider additional configs
        /// </summary>
        public ProviderAdditionalConfigDto[] AdditionalConfigs { get; set; }
    }
}
