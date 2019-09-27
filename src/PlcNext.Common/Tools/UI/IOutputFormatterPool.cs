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
using System.Globalization;
using System.Linq;
using System.Text;

namespace PlcNext.Common.Tools.UI
{
    public interface IOutputFormatterPool
    {
        IUserInterface GetFormatter(FormatterParameters parameters, IUserInterface wrappedUserInterface);
    }

    public class FormatterParameters
    {
        private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

        public bool TryGet<T>(out T value)
        {
            value = parameters.Values.OfType<T>().FirstOrDefault();
            return !Equals(value, default(T));
        }

        public bool TryGet<T>(string name, out T value)
        {
            bool exist = parameters.ContainsKey(name) && parameters[name] is T;
            value = exist ? (T) parameters[name] : default(T);
            return exist;
        }

        public void Add<T>(T value)
        {
            parameters[Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture)] = value;
        }

        public void Add<T>(T value, string name)
        {
            parameters[name] = value;
        }
    }
}
