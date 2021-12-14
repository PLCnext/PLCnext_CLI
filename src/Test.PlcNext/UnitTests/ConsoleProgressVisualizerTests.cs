#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using PlcNext.Common.Tools.UI;
using Xunit;

namespace Test.PlcNext.UnitTests
{
    public class ConsoleProgressVisualizerTests
    {
        private const string SetPrefixMethodName = "SetPrefix";
        private const string WriteInformationMethodName = "WriteInformation";
        private const string PrefixArrow = "-> ";
        private const string PrefixWhiteSpace = "   ";
        
        [Fact]
        public void ProgressNotifierTicksCorrectly()
        {
            IUserInterface userInterfaceSubstitute = Substitute.For<IUserInterface>();
            ConsoleProgressVisualizer visualizer =
                new ConsoleProgressVisualizer(userInterfaceSubstitute);

            IProgressNotifier progressNotifier = visualizer.Spawn(100.0, "StartMessage", "CompleteMessage");
            for(int i = 10; i <= 100;i+=10)
                progressNotifier.TickIncrement(10);
                
            IEnumerable<string> messages = userInterfaceSubstitute
                .ReceivedCalls()
                .Where(x => x.GetMethodInfo().Name.Equals(nameof(userInterfaceSubstitute.WriteInformation)))
                .Select(x => x.GetArguments()[0] as string);

            messages.Should().BeEquivalentTo(new List<string>()
            {
                "[0%] StartMessage",
                "[10%] StartMessage",
                "[20%] StartMessage",
                "[30%] StartMessage",
                "[40%] StartMessage",
                "[50%] StartMessage",
                "[60%] StartMessage",
                "[70%] StartMessage",
                "[80%] StartMessage",
                "[90%] StartMessage",
                "[100%] StartMessage",
                "CompleteMessage",
            });
        }
        
        [Fact]
        public void ProgressNotifierTicksWithMessagesCorrectly()
        {
            IUserInterface userInterfaceSubstitute = Substitute.For<IUserInterface>();
            ConsoleProgressVisualizer visualizer =
                new ConsoleProgressVisualizer(userInterfaceSubstitute);

            IProgressNotifier progressNotifier = visualizer.Spawn(100.0, "StartMessage", "CompleteMessage");
            for(int i = 10; i <= 100;i+=10)
                progressNotifier.TickIncrement(10, i.ToString());
                
            IEnumerable<string> messages = userInterfaceSubstitute
                .ReceivedCalls()
                .Where(x => x.GetMethodInfo().Name.Equals(nameof(userInterfaceSubstitute.WriteInformation)))
                .Select(x => x.GetArguments()[0] as string);

            messages.Should().BeEquivalentTo(new List<string>()
            {
                "[0%] StartMessage",
                "[10%] StartMessage: 10",
                "[20%] StartMessage: 20",
                "[30%] StartMessage: 30",
                "[40%] StartMessage: 40",
                "[50%] StartMessage: 50",
                "[60%] StartMessage: 60",
                "[70%] StartMessage: 70",
                "[80%] StartMessage: 80",
                "[90%] StartMessage: 90",
                "[100%] StartMessage: 100",
                "CompleteMessage",
            });
        }
        
        [Fact]
        public async Task ProgressInfiniteNotifierTicksCorrectly()
        {
            IUserInterface userInterfaceSubstitute = Substitute.For<IUserInterface>();
            
            List<(string, object[])> calls = new List<(string, object[])>();
            void ProcessCall(string methodName, object[] arguments) => calls.Add((methodName, arguments));

            userInterfaceSubstitute
                .WhenForAnyArgs(x => x.SetPrefix(Arg.Any<string>()))
                .Do(x => ProcessCall(SetPrefixMethodName, x.Args()));
            
            userInterfaceSubstitute
                .WhenForAnyArgs(x => x.WriteInformation(Arg.Any<string>()))
                .Do(x => ProcessCall(WriteInformationMethodName, x.Args()));
            
            ConsoleProgressVisualizer visualizer =
                new ConsoleProgressVisualizer(userInterfaceSubstitute);

            IDisposable progressNotifier = visualizer.SpawnInfiniteProgress("StartMessage");
            
            await Task.Delay(2500); 
            
            progressNotifier.Dispose();
            
            string firstDepthPrefix = $"{PrefixWhiteSpace}{PrefixArrow}";
            
            calls.Should().BeEquivalentTo(new List<(string, object[])>()
            {
                (SetPrefixMethodName, new object[] {firstDepthPrefix}),
                
                (SetPrefixMethodName, new object[] {string.Empty}),
                (WriteInformationMethodName, new object[] {"[Infinite] StartMessage", true}),
                (SetPrefixMethodName, new object[]{firstDepthPrefix}),
                
                (SetPrefixMethodName, new object[]{firstDepthPrefix}),
                (WriteInformationMethodName,new object[]{".", false}),
                
                (SetPrefixMethodName, new object[]{firstDepthPrefix}),
                (WriteInformationMethodName,new object[]{".", false}),
                
                (SetPrefixMethodName, new object[] {string.Empty}),
                (WriteInformationMethodName, new object[] {"[Infinite] StartMessage: Done", true}),
                (SetPrefixMethodName, new object[]{firstDepthPrefix}),
            });
        }

        [Fact]
        public void SubProgressNotifierCorrectIndentationAndParentTickWhenComplete()
        {
            IUserInterface userInterfaceSubstitute = Substitute.For<IUserInterface>();

            List<(string, object[])> calls = new List<(string, object[])>();
            void ProcessCall(string methodName, object[] arguments) => calls.Add((methodName, arguments));

            userInterfaceSubstitute
                .WhenForAnyArgs(x => x.SetPrefix(Arg.Any<string>()))
                .Do(x => ProcessCall(SetPrefixMethodName, x.Args()));

            userInterfaceSubstitute
                .WhenForAnyArgs(x => x.WriteInformation(Arg.Any<string>()))
                .Do(x => ProcessCall(WriteInformationMethodName, x.Args()));

            ConsoleProgressVisualizer visualizer =
                new ConsoleProgressVisualizer(userInterfaceSubstitute);

            IProgressNotifier progressNotifier = visualizer.Spawn(100.0, "StartMessage", "CompleteMessage");


            progressNotifier.TickIncrement(9);
            IProgressNotifier childNotifier = progressNotifier.Spawn(100.0, "ChildStartMessage");

            for (int i = 0; i < 5; i++)
                childNotifier.TickIncrement(20);

            progressNotifier.Tick(100);

            string firstDepthPrefix = $"{PrefixWhiteSpace}{PrefixArrow}";
            string secondDepthPrefix = $"{PrefixWhiteSpace}{firstDepthPrefix}";

            calls.Should().BeEquivalentTo(new List<(string, object[])>()
            {
                (SetPrefixMethodName, new object[] {firstDepthPrefix}),

                (SetPrefixMethodName, new object[] {string.Empty}),
                (WriteInformationMethodName, new object[] {"[0%] StartMessage", true}),
                (SetPrefixMethodName, new object[] {firstDepthPrefix}),

                (SetPrefixMethodName, new object[] {string.Empty}),
                (WriteInformationMethodName, new object[] {"[9%] StartMessage", true}),
                (SetPrefixMethodName, new object[] {firstDepthPrefix}),

                (SetPrefixMethodName, new object[] {secondDepthPrefix}),

                (SetPrefixMethodName, new object[] {firstDepthPrefix}),
                (WriteInformationMethodName, new object[] {"[0%] ChildStartMessage", true}),
                (SetPrefixMethodName, new object[] {secondDepthPrefix}),

                (SetPrefixMethodName, new object[] {firstDepthPrefix}),
                (WriteInformationMethodName, new object[] {"[20%] ChildStartMessage", true}),
                (SetPrefixMethodName, new object[] {secondDepthPrefix}),

                (SetPrefixMethodName, new object[] {firstDepthPrefix}),
                (WriteInformationMethodName, new object[] {"[40%] ChildStartMessage", true}),
                (SetPrefixMethodName, new object[] {secondDepthPrefix}),

                (SetPrefixMethodName, new object[] {firstDepthPrefix}),
                (WriteInformationMethodName, new object[] {"[60%] ChildStartMessage", true}),
                (SetPrefixMethodName, new object[] {secondDepthPrefix}),

                (SetPrefixMethodName, new object[] {firstDepthPrefix}),
                (WriteInformationMethodName, new object[] {"[80%] ChildStartMessage", true}),
                (SetPrefixMethodName, new object[] {secondDepthPrefix}),

                (SetPrefixMethodName, new object[] {firstDepthPrefix}),
                (WriteInformationMethodName, new object[] {"[100%] ChildStartMessage", true}),
                (SetPrefixMethodName, new object[] {secondDepthPrefix}),

                (SetPrefixMethodName, new object[] {string.Empty}),
                (WriteInformationMethodName, new object[] {"[10%] StartMessage", true}),
                (SetPrefixMethodName, new object[] {firstDepthPrefix}),

                (SetPrefixMethodName, new object[] {string.Empty}),
                (WriteInformationMethodName, new object[] {"[100%] StartMessage", true}),
                (SetPrefixMethodName, new object[] {firstDepthPrefix}),

                (SetPrefixMethodName, new object[] {string.Empty}),
                (WriteInformationMethodName, new object[] {"CompleteMessage", true})
            });
        }

        [Fact]
        public async Task InfiniteSubProgressTicksWithCorrectIndentationAndParentTickOnComplete()
        {
            IUserInterface userInterfaceSubstitute = Substitute.For<IUserInterface>();
            
            List<(string, object[])> calls = new List<(string, object[])>();
            void ProcessCall(string methodName, object[] arguments) => calls.Add((methodName, arguments));

            userInterfaceSubstitute
                .WhenForAnyArgs(x => x.SetPrefix(Arg.Any<string>()))
                .Do(x => ProcessCall(SetPrefixMethodName, x.Args()));
            
            userInterfaceSubstitute
                .WhenForAnyArgs(x => x.WriteInformation(Arg.Any<string>()))
                .Do(x => ProcessCall(WriteInformationMethodName, x.Args()));
            
            ConsoleProgressVisualizer visualizer =
                new ConsoleProgressVisualizer(userInterfaceSubstitute);

            IProgressNotifier progressNotifier = visualizer.Spawn(100.0, "StartMessage", "CompleteMessage");


            progressNotifier.TickIncrement(9);

            IDisposable childChildInfiniteNotifier = progressNotifier.SpawnInfiniteProgress("ChildStartMessage");
            
            await Task.Delay(2500); 
            
            childChildInfiniteNotifier.Dispose();
            
            progressNotifier.Tick(100);

            string firstDepthPrefix = $"{PrefixWhiteSpace}{PrefixArrow}";
            string secondDepthPrefix = $"{PrefixWhiteSpace}{firstDepthPrefix}";
            string thirdDepthPrefix = $"{PrefixWhiteSpace}{secondDepthPrefix}";

            calls.Should().BeEquivalentTo(new List<(string, object[])>()
            {
                (SetPrefixMethodName, new object[]{firstDepthPrefix}),
                
                (SetPrefixMethodName, new object[]{string.Empty}),
                (WriteInformationMethodName,new object[]{"[0%] StartMessage", true}),
                (SetPrefixMethodName, new object[]{firstDepthPrefix}),
                
                (SetPrefixMethodName, new object[]{string.Empty}),
                (WriteInformationMethodName,new object[]{"[9%] StartMessage", true}),
                (SetPrefixMethodName, new object[]{firstDepthPrefix}),
                
                (SetPrefixMethodName, new object[]{secondDepthPrefix}),
                
                (SetPrefixMethodName, new object[]{firstDepthPrefix}),
                (WriteInformationMethodName,new object[]{"[Infinite] ChildStartMessage", true}),
                (SetPrefixMethodName, new object[]{secondDepthPrefix}),
                
                (SetPrefixMethodName, new object[]{secondDepthPrefix}),
                (WriteInformationMethodName,new object[]{".", false}),
                
                (SetPrefixMethodName, new object[]{secondDepthPrefix}),
                (WriteInformationMethodName,new object[]{".", false}),
                
                (SetPrefixMethodName, new object[]{firstDepthPrefix}),
                (WriteInformationMethodName,new object[]{"[Infinite] ChildStartMessage: Done", true}),
                (SetPrefixMethodName, new object[]{secondDepthPrefix}),
                
                (SetPrefixMethodName, new object[]{string.Empty}),
                (WriteInformationMethodName,new object[]{"[10%] StartMessage", true}),
                (SetPrefixMethodName, new object[]{firstDepthPrefix}),
                
                (SetPrefixMethodName, new object[]{string.Empty}),
                (WriteInformationMethodName,new object[]{"[100%] StartMessage", true}),
                (SetPrefixMethodName, new object[]{firstDepthPrefix}),
                
                (SetPrefixMethodName, new object[]{string.Empty}),
                (WriteInformationMethodName,new object[]{"CompleteMessage", true})
            });
        }
    }
}