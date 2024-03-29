﻿using System.Collections.Concurrent;
using Assignment.DosProtection.DM.Enum;
using Assignment.DosProtection.DM.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Assignment.DosProtection.DM.Models
{
    public class DosProtectionService : IDosProtectionService
    {
        private readonly ILogger<DosProtectionService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, IDosProtectionClient> _staticWindowClients = new ConcurrentDictionary<string, IDosProtectionClient>();
        private readonly ConcurrentDictionary<string, IDosProtectionClient> _dynamicWindowClients = new ConcurrentDictionary<string, IDosProtectionClient>();

        public DosProtectionService(IMemoryCache memoryCache, IServiceProvider serviceProvider,
            ILogger<DosProtectionService> logger)
        {
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <summary>
        /// Checks if the client can make another request within the specified time frame
        /// based on the client's ID and IP address.
        /// </summary>
        /// <returns>
        ///   True if the client is allowed to make another request within the defined limits; otherwise, false.
        /// </returns>
        public bool ProcessClientRequest(string clientId, string clientIpAddress, ProtectionType protectionType)
        {
            _logger.LogError($"Request: {clientId}, Thread: {Thread.CurrentThread.ManagedThreadId}");
            _logger.LogDebug($"[DosProtectionService:ProcessClientRequest] Starts processing the request of client ID: {clientId} with IP address: {clientIpAddress}.");
           
            // Get the relevant concurrent dictionary based on the protection type.
            var windowClients = protectionType == ProtectionType.Static ? _staticWindowClients : _dynamicWindowClients;

            // Get or add a DosProtectionClient instance for the clientId from the relevant concurrent dictionary.
            var dosClient = windowClients.GetOrAdd(clientId, entry => _serviceProvider.GetRequiredService<IDosProtectionClient>());

            // Get or add a DosProtectionClient instance for the client's IP address from cache.
            var dosClientIp = _memoryCache.GetOrCreate(clientIpAddress, entry => _serviceProvider.GetRequiredService<IDosProtectionClient>());

            // Check if the client is allowed to make another request.
            return dosClient.CheckRequestRate(protectionType)/* && dosClientIp.CheckRequestRate(protectionType)*/;
        }
    }
}
