// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BusyBox.AspNetCore.Json;

namespace BusyBox.AspNetCore.Benchmarks
{
    [MemoryDiagnoser]
    public class NamingPolicy
    {
        [ParamsSource(nameof(GetValues))] public string PropertyName { get; set; }

        [Benchmark]
        public void ShakeCaseNaming()
        {
            var policy = new SnakeCaseNamingPolicy();
            policy.ConvertName(PropertyName);
        }

        public static IEnumerable<string> GetValues()
        {
            var random = new Random();
            var lengths = new[] { 10, 100, 1000 };
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            foreach (int length in lengths)
            {
                var charset = new char[length];
                for (int index = 0; index < length; index++)
                {
                    charset[index] = chars[random.Next(chars.Length - 1)];
                }

                yield return new string(charset);
            }
        }
    }
}
