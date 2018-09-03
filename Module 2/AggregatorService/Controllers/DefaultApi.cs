using System;
using System.ComponentModel.DataAnnotations;

using AggregatorService.Attributes;
using AggregatorService.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using Polly;
using ServiceClients;
using ServiceStore;

using Swashbuckle.AspNetCore.Annotations;

namespace AggregatorService.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultApiController : Controller
    {
        private const string GuidRegex = "^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$";

        private readonly IStore store;
        private readonly ILogger logger;
        private readonly ITemperatureHistorian historian;

        public DefaultApiController(IStore store, ILogger<DefaultApiController> logger, ITemperatureHistorian historian)
        {
            this.store = store;
            this.logger = logger;
            this.historian = historian;
        }

        /// <summary>
        /// Add data generated from a device to the aggregator
        /// </summary>
        /// <remarks>Adds a data point from an IoT device. The aggregator selects the historian service, posts data to it, and receives the running average. Then updates its store for the history of running averages by device id and type.</remarks>
        /// <param name="deviceType">Device type</param>
        /// <param name="deviceId">Device ID</param>
        /// <param name="dataPointId">Each data point needs to have a unique ID</param>
        /// <param name="value">Value registered by the device.</param>
        /// <response code="201">Data added successfully.</response>
        /// <response code="401">Invalid input parameter.</response>
        /// <response code="500">An unexpected error occurred.</response>
        [HttpPost]
        [Route("/v1/deviceData/{deviceType}/{deviceId}")]
        [ValidateModelState]
        [SwaggerOperation("AddDeviceData")]
        [SwaggerResponse(401, type: typeof(Error), description: "Invalid input parameter.")]
        [SwaggerResponse(500, type: typeof(Error), description: "An unexpected error occurred.")]
        public virtual IActionResult AddDeviceData(
            [FromRoute][Required] string deviceType,
            [FromRoute][Required][RegularExpression(GuidRegex)][StringLength(36, MinimumLength = 36)] string deviceId,
            [FromQuery][Required][RegularExpression(GuidRegex)][StringLength(36, MinimumLength = 36)] string dataPointId,
            [FromQuery][Required] float? value)
        {
             if (!deviceType.Equals("TEMP"))
             {
                 this.logger.LogError($"Device type {deviceType} is not supported.");
                 return BadRequest($"Unsupported device type {deviceType}");
             }
             float? averageValue = default(float?);

             var retryPolicy = Policy
                 .Handle<HttpOperationException>()
                 .WaitAndRetry(5, retryAttempt =>
                     TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                 );

             averageValue = retryPolicy.Execute(() =>
               this.historian.AddDeviceData(deviceId, dataPointId, DateTimeOffset.UtcNow.DateTime, value));

             if (!averageValue.HasValue)
             {
                 var message = $"Cannot calculate the average.";
                 this.logger.LogError(message);
                 return BadRequest(message);
             }

             var key = $"{deviceType};{deviceId}";
             if (this.store.Exists(key))
             {
                 this.logger.LogInformation($"Updating {key} with {averageValue.Value}");
                 this.store.Update(key, averageValue.Value);
             }
             else
             {
                 this.logger.LogInformation($"Added {key} with {averageValue.Value}");
                 this.store.Add(key, averageValue.Value);
             }

             return Ok(averageValue.Value);
        }

        /// <summary>
        /// Get the running averages of a device type given a date range.
        /// </summary>
        /// <remarks>Returns the running average of a device type given a date range, averaged by the minute.</remarks>
        /// <param name="deviceType">Device type</param>
        /// <param name="fromTime">Start of the date range.</param>
        /// <param name="toTime">End of the date range.</param>
        /// <response code="200">Running averages per minute</response>
        /// <response code="400">Invalid input parameter.</response>
        /// <response code="500">An unexpected error occurred.</response>
        [HttpGet]
        [Route("/v1/averageByDeviceType/{deviceType}")]
        [ValidateModelState]
        [SwaggerOperation("AverageByDeviceTypeDeviceTypeGet")]
        [SwaggerResponse(200, type: typeof(DeviceDataPoints), description: "Running averages per minute")]
        [SwaggerResponse(400, type: typeof(Error), description: "Invalid input parameter.")]
        [SwaggerResponse(500, type: typeof(Error), description: "An unexpected error occurred.")]
        public virtual IActionResult AverageByDeviceTypeDeviceTypeGet(
            [FromRoute][Required]string deviceType,
            [FromQuery][Required]DateTime? fromTime,
            [FromQuery][Required]DateTime? toTime)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(DeviceDataPoints));

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400, default(Error));

            //TODO: Uncomment the next line to return response 500 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(500, default(Error));

            var exampleJson = "\"\"";

            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<DeviceDataPoints>(exampleJson)
            : default(DeviceDataPoints);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}
