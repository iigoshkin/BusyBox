using System;
using BenchmarkDotNet.Running;

namespace BusyBox.AspNetCore.Benchmarks
{
	class Program
	{
		static void Main(string[] args)
		{
            var summary = BenchmarkRunner.Run<NamingPolicy>();
		}
	}
}
