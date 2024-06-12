using BEPUphysics.Entities.Prefabs;
using BEPUutilities;

namespace BEPUfloatBenchmark.Benchmarks
{
    public class PyramidBenchmark : Benchmark
    {
        protected override void InitializeSpace()
        {
            float boxSize = 2;
            int boxCount = 20;
            float platformLength = (float)MathHelper.Min(50, boxCount * boxSize + 10);
            Space.Add(new Box(new Vector3(0, -.5f, 0), boxCount * boxSize + 20, 1,
                              platformLength));

            for (int i = 0; i < boxCount; i++)
            {
                for (int j = 0; j < boxCount - i; j++)
                {
                    Space.Add(new Box(
                                  new Vector3(
                                      -boxCount * boxSize / 2 + boxSize / 2 * i + j * (boxSize),
                                      (boxSize / 2) + i * boxSize,
                                      0),
                                  boxSize, boxSize, boxSize, 20));
                }
            }
            //Down here are the 'destructors' used to blow up the pyramid.

            Sphere pow = new Sphere(new Vector3(-25, 5, 70), 2, 40);
            pow.LinearVelocity = new Vector3(0, 10, -100);
            Space.Add(pow);
            pow = new Sphere(new Vector3(-15, 10, 70), 2, 40);
            pow.LinearVelocity = new Vector3(0, 10, -100);
            Space.Add(pow);
            pow = new Sphere(new Vector3(-5, 15, 70), 2, 40);
            pow.LinearVelocity = new Vector3(0, 10, -100);
            Space.Add(pow);
            pow = new Sphere(new Vector3(5, 15, 70), 2, 40);
            pow.LinearVelocity = new Vector3(0, 10, -100);
            Space.Add(pow);
            pow = new Sphere(new Vector3(15, 10, 70), 2, 40);
            pow.LinearVelocity = new Vector3(0, 10, -100);
            Space.Add(pow);
            pow = new Sphere(new Vector3(25, 5, 70), 2, 40);
            pow.LinearVelocity = new Vector3(0, 10, -100);
            Space.Add(pow);
        }
    }
}
