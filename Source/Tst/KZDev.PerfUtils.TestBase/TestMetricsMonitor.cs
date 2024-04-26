// Copyright (c) Kevin Zehrer
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Diagnostics.Tracing;

namespace KZDev.PerfUtils.Tests
{
    /// <summary>
    /// A helper class that listens to the EventSource events and provides the metrics as
    /// needed for testing.
    /// </summary>
    public class TestMetricsMonitor : EventListener
    {
        /// <summary>
        /// The number of times the update event has been called.
        /// </summary>
        private volatile int _updateCount;

        /// <summary>
        /// The size of the Gen0 heap.
        /// </summary>
        private double _gen0Size;

        /// <summary>
        /// The size of the Gen1 heap.
        /// </summary>
        private double _gen1Size;

        /// <summary>
        /// The size of the Gen2 heap.
        /// </summary>
        private double _gen2Size;

        /// <summary>
        /// The size of the LOH heap.
        /// </summary>
        private double _lohSize;

        /// <summary>
        /// Gets the most recent LOH size.
        /// </summary>
        public double LohSize
        {
            get
            {
                // This is not an efficient way to get the LOH size, but it is good enough for testing.
                int updateCountSnapshot = _updateCount;
                while (updateCountSnapshot == _updateCount)
                {
                    Thread.Sleep(50);
                }
                return _lohSize;
            }
        }

        /// <inheritdoc />
        protected override void OnEventSourceCreated (EventSource source)
        {
            // Currently, we are only interested in the System.Runtime event source
            if (!source.Name.Equals("System.Runtime"))
            {
                return;
            }

            // Get event updated every second
            EnableEvents(source, EventLevel.Verbose, EventKeywords.All, new Dictionary<string, string?>()
            {
                ["EventCounterIntervalSec"] = "1"
            });
        }

        /// <inheritdoc />
        protected override void OnEventWritten (EventWrittenEventArgs eventData)
        {
            if (eventData.EventName is null)
                return;
            if (!eventData.EventName.Equals("EventCounters"))
            {
                return;
            }
            if (eventData.Payload is null)
            {
                return;
            }

            for (int payloadIndex = 0; payloadIndex < eventData.Payload.Count; ++payloadIndex)
            {
                if (eventData.Payload[payloadIndex] is not IDictionary<string, object> eventPayload)
                    continue;
                (string counterName, object? counterValue) = GetRelevantMetric(eventPayload);
                switch (counterName)
                {
                    case "gen-0-size":
                        _gen0Size = (double)counterValue!;
                        break;
                    case "gen-1-size":
                        _gen1Size = (double)counterValue!;
                        break;
                    case "gen-2-size":
                        _gen2Size = (double)counterValue!;
                        break;
                    case "loh-size":
                        _lohSize = (double)counterValue!;
                        break;
                }
            }
            Interlocked.Increment(ref _updateCount);
        }

        private static (string counterName, object? counterValue) GetRelevantMetric (
            IDictionary<string, object> eventPayload)
        {
            string counterName = "";
            object? counterValue = null;

            if (eventPayload.TryGetValue("Name", out object? nameValue))
            {
                counterName = nameValue.ToString() ?? string.Empty;
            }
            if ((eventPayload.TryGetValue("CounterType", out object? counterType) &&
                (counterType is string countTypeName) && 
                 eventPayload.TryGetValue(countTypeName, out object? value)))
            {
                counterValue = value;
            }
            if ((counterValue is null) && (eventPayload.TryGetValue("Count", out object? countValue) && (countValue is not null)))
            {
                if (countValue is long longCount)
                {
                    counterValue = longCount;
                }
                if (countValue is int intCount)
                {
                    counterValue = intCount;
                }
                if (countValue is double doubleCount)
                {
                    counterValue = doubleCount;
                }
            }
            if ((counterValue is null) &&
                (eventPayload.TryGetValue("Mean", out value) ||
                              eventPayload.TryGetValue("Increment", out value)))
            {
                counterValue = value;
            }

            return (counterName, counterValue);
        }
    }
}
