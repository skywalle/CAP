﻿using System;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using DotNetCore.CAP.Processor.States;
using Microsoft.Extensions.Logging;

namespace DotNetCore.CAP.Kafka
{
    internal class PublishQueueExecutor : BasePublishQueueExecutor
    {
        private readonly KafkaOptions _kafkaOptions;
        private readonly ILogger _logger;

        public PublishQueueExecutor(
            CapOptions options,
            IStateChanger stateChanger,
            KafkaOptions kafkaOptions,
            ILogger<PublishQueueExecutor> logger)
            : base(options, stateChanger, logger)
        {
            _logger = logger;
            _kafkaOptions = kafkaOptions;
        }

        public override Task<OperateResult> PublishAsync(string keyName, string content)
        {
            try
            {
                var config = _kafkaOptions.AsKafkaConfig();
                var contentBytes = Encoding.UTF8.GetBytes(content);
                using (var producer = new Producer(config))
                {
                    var message = producer.ProduceAsync(keyName, null, contentBytes).Result;

                    if (!message.Error.HasError)
                    {
                        _logger.LogDebug($"kafka topic message [{keyName}] has been published.");

                        return Task.FromResult(OperateResult.Success);
                    }
                    return Task.FromResult(OperateResult.Failed(new OperateError
                    {
                        Code = message.Error.Code.ToString(),
                        Description = message.Error.Reason
                    }));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"An error occurred during sending the topic message to kafka. Topic:[{keyName}], Exception: {ex.Message}");

                return Task.FromResult(OperateResult.Failed(ex));
            }
        }
    }
}