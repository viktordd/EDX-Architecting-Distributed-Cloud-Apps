using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using ServiceStore;

using Swashbuckle.AspNetCore.Annotations;

using TemperatureHistorianService.Attributes;
using TemperatureHistorianService.Models;

namespace TemperatureHistorianService.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultApiController : Controller
    {
        private const string GuidRegex = "^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$";

        private readonly IStore store;
        private readonly ILogger logger;

        public DefaultApiController(IStore store, ILogger<DefaultApiController> logger)
        {
            this.store = store;
            this.logger = logger;
        }

        /// <summary>
        /// Add data to the device history
        /// </summary>
        /// <remarks>Adds a data point from an IoT device. Once saved, calculates the running average of the existing data, saves it idempotentently and returns it.</remarks>
        /// <param name="deviceId">Device Id</param>
        /// <param name="datapointId">Each data point needs to have a unique ID</param>
        /// <param name="timestamp">Timestamp when received from the device.</param>
        /// <param name="value">Value registered by the device.</param>
        /// <response code="201">Data added successfully.</response>
        /// <response code="400">Invalid input parameter.</response>
        /// <response code="500">An unexpected error occurred.</response>
        [HttpPost]
        [Route("/v1/deviceData/{deviceId}")]
        [ValidateModelState]
        [SwaggerOperation("AddDeviceData")]
        [SwaggerResponse(201, type: typeof(float?), description: "Data added successfully.")]
        [SwaggerResponse(400, type: typeof(Error), description: "Invalid input parameter.")]
        [SwaggerResponse(500, type: typeof(Error), description: "An unexpected error occurred.")]
        public virtual IActionResult AddDeviceData(
            [FromRoute][Required][RegularExpression(GuidRegex)][StringLength(36, MinimumLength = 36)]string deviceId,
            [FromQuery][Required][RegularExpression(GuidRegex)][StringLength(36, MinimumLength = 36)]string datapointId,
            [FromQuery][Required]DateTime? timestamp,
            [FromQuery][Required]float? value)
        {
            var key = $"{deviceId};{datapointId}";

            if (!store.Exists(key) && value.HasValue)
            {
                store.Add(key, value.Value);
                logger.LogInformation($"Added {value.Value} for {key} to the store at {timestamp}.");
            }

            if (!value.HasValue)
            {
                logger.LogError($"No value found for {key}.");
                return BadRequest($"No data value for device: {deviceId} and datapoint {datapointId}");
            }

            var average = store.GetAll().Where(i => i.Key.StartsWith(deviceId)).Average(v => v.Value);

            logger.LogInformation($"Returning {average}.");
            return Created("", average);
        }
    }
}
