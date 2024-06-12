using BEPUphysics;
using BEPUphysics.Entities;
using BEPUutilities;
using System;
using System.Security.Cryptography;
using System.Text;

namespace BEPUfloatBenchmark
{
	public abstract class Benchmark
	{
		protected Space Space;
		
		protected abstract void InitializeSpace();
		protected virtual void Step()
		{
			Space.Update();
		}

		public void Initialize()
		{
			Space = new Space();
			Space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);
			Space.TimeStepSettings.TimeStepDuration = 1f / 60;

			InitializeSpace();
		}

		public void Dispose()
		{
			Space = null;
		}

		public double RunBenchmark()
		{
			Console.WriteLine("");
			long startTime = DateTime.Now.Ticks;
			long opStartTime = DateTime.Now.Ticks;
			int opCount = 0;
			for (int i = 0; i < 1000; i++)
			{
				Step();
				opCount++;
				long time = DateTime.Now.Ticks - opStartTime;
				if (time > TimeSpan.TicksPerSecond)
				{
					Console.Write(string.Format("\rAvg. duration per Step: {0}ms                ", (time/TimeSpan.TicksPerMillisecond)/opCount));
					opCount = 0;
					opStartTime = DateTime.Now.Ticks;
				}					
			}

			long runTime = (DateTime.Now.Ticks - startTime);

			Console.Write("\r                                                                          \r");
			return (double)runTime / TimeSpan.TicksPerSecond;
		}

		public string GetName()
		{
			return this.GetType().Name;
		}
	}
}
