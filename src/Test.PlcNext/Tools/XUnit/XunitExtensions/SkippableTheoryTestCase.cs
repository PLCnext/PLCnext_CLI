#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Test.PlcNext.Tools.XUnit.XunitExtensions
{
    public class SkippableTheoryTestCase : XunitTheoryTestCase
    {
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public SkippableTheoryTestCase() { }

        public SkippableTheoryTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod)
            : base(diagnosticMessageSink, defaultMethodDisplay, TestMethodDisplayOptions.None, testMethod) { }

        public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
                                                        IMessageBus messageBus,
                                                        object[] constructorArguments,
                                                        ExceptionAggregator aggregator,
                                                        CancellationTokenSource cancellationTokenSource)
        {
            // Duplicated code from SkippableFactTestCase. I'm sure we could find a way to de-dup with some thought.
            var skipMessageBus = new SkippableFactMessageBus(messageBus);
            var result = await base.RunAsync(diagnosticMessageSink, skipMessageBus, constructorArguments, aggregator, cancellationTokenSource);
            if (skipMessageBus.DynamicallySkippedTestCount > 0)
            {
                result.Failed -= skipMessageBus.DynamicallySkippedTestCount;
                result.Skipped += skipMessageBus.DynamicallySkippedTestCount;
            }

            return result;
        }
    }
}
