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
using System.Text;
using NSubstitute;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.UI;
using Test.PlcNext.SystemTests.Tools;

namespace Test.PlcNext.Tools.Abstractions.Mocked
{
    internal class MockedExceptionHandlerAbstraction : IExceptionHandlerAbstraction
    {
        private readonly IExceptionHandler exceptionHandler = Substitute.For<IExceptionHandler>();
        private readonly HashSet<Exception> recievedExceptions = new HashSet<Exception>();
        
        public MockedExceptionHandlerAbstraction()
        {
            exceptionHandler.HandleException(Arg.Any<Exception>())
                            .Returns(info => HandleException(info.Arg<Exception>()));

            bool HandleException(Exception e)
            {
                recievedExceptions.Add(e);
                Exception inner = e.InnerException;
                while (inner != null)
                {
                    recievedExceptions.Add(inner);
                    UserInterface?.WriteError($"-> {inner.Message}");
                    inner = inner.InnerException;
                }

                switch (e)
                {
                    case FormattableException formattableException:
                        UserInterface?.WriteError(formattableException.Message);
                        return true;
                    case AggregateException aggregateException when aggregateException.InnerExceptions.All(ie => ie is FormattableException):
                        foreach (Exception innerException in aggregateException.InnerExceptions)
                        {
                            UserInterface?.WriteError(innerException.Message);
                        }
                        return true;
                }

                return false;
            }
        }

        public void Dispose()
        {
            
        }

        public void Initialize(InstancesRegistrationSource exportProvider)
        {
            exportProvider.AddInstance(exceptionHandler);
        }

        public IUserInterface UserInterface { get; set; }

        public bool WasExceptionThrown(Type exceptionType)
        {
            return recievedExceptions.Any(exceptionType.IsInstanceOfType) ||
                   recievedExceptions.OfType<AggregateException>()
                                     .Any(a => a.InnerExceptions.All(exceptionType.IsInstanceOfType) &&
                                               a.InnerExceptions.Count == 1);
        }
    }
}
